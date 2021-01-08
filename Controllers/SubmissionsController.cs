﻿using IEIPaperSearch.Models;
using IEIPaperSearch.Services.DataLoaders;
using IEIPaperSearch.Services.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace IEIPaperSearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        readonly ISearchService searchService;
        readonly IDataLoaderService loaderService;

        public SubmissionsController(ISearchService searchService, IDataLoaderService loaderService)
        {
            this.searchService = searchService;
            this.loaderService = loaderService;
        }

        /// <summary>
        /// Search submissions by author or title and then optionally filter the results by publication year.
        /// </summary>
        /// <remarks>
        /// All fields are optional, but you must at least provide an author query or a title query. Leaving all fields blank is
        /// not allowed. At least one type of submission (article, book or conference proceedings) must be selected.
        /// </remarks>
        /// <param name="author">The name or surnames of the author to search, or part of them.</param>
        /// <param name="title">The title of the submission to search, or part of it.</param>
        /// <param name="startingYear">The minimum (inclusive) publication year to include.</param>
        /// <param name="endYear">The maximum (inclusive) publication year to include.</param>
        /// <param name="findArticles">Set to true to include articles.</param>
        /// <param name="findBooks">Set to true to include books.</param>
        /// <param name="findInProceedings">Set to true to include conference proceedings.</param>
        /// <response code="200">A collection of all submissions matching the search criteria, or an empty collection if no result was found.</response>
        /// <response code="400">If the search query or conditions are invalid.</response>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<Submission>> Search(string? author, string? title, uint? startingYear, uint? endYear, bool findArticles, bool findBooks, bool findInProceedings)
        {
            if (author is null && title is null)
            {
                return BadRequest("Include at least one author or title query.");
            }
            if (!findArticles && !findBooks && !findInProceedings)
            {
                return BadRequest("At least one of articles, books or conference proceedings must be selected.");
            }
            if (startingYear is not null && endYear is not null && endYear < startingYear)
            {
                return BadRequest("End year cannot be before starting year.");
            }

            var results = searchService.Search(title, author, (int?)startingYear, (int?)endYear, findArticles, findBooks, findInProceedings);
            return Ok(results);
        }

        /// <summary>
        /// Extract data from known external sources and insert it into the local persistent storage, filtered by publication
        /// year.
        /// </summary>
        /// <remarks>
        /// This procedure will normalize, consolidate and deduplicate data from the different diverging schemes of each
        /// external source into the common scheme of the local domain, populating it with varied data.
        /// This procedure is to be considered a maintenance downtime routine, as it may take a long time and severely impact 
        /// the performance of the local persistent storage.
        /// Non-critical errors that occur during any point of this operation will be collected and returned at the end. Hence,
        /// this operation will silently skip errors and may finish successfully after having partially or totally ignored some 
        /// of the selected sources. The caller is advised to examine the resulting error log.
        /// </remarks>
        /// <param name="startingYear">The minimum (inclusive) publication year to include.</param>
        /// <param name="endYear">The maximum (inclusive) publication year to include.</param>
        /// <param name="useDblp">Set to true to include submissions from DBLP static XML data.</param>
        /// <param name="useIeeeXplore">Set to true to include submissions from the IEEE Xplore REST API.</param>
        /// <param name="useGoogleScholar">Set to true to include submissions scraped from the Google Scholar website.</param>
        /// <response code="200">If the operation has finished. A set of diagnostic data is returned, including the error log.</response>
        /// <response code="400">If the input parameters are invalid.</response>
        /// <response code="500">If a critical error has prevented the operation from finishing.</response>
        [HttpPut("loadFromExternalSources")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IDataLoaderService.DataLoaderResult> LoadFromExternalSources(uint startingYear, uint endYear, bool useDblp, bool useIeeeXplore, bool useGoogleScholar)
        {
            if (!useDblp && !useIeeeXplore && !useGoogleScholar)
            {
                return BadRequest("At least one external source must be selected.");
            }
            if (endYear < startingYear)
            {
                return BadRequest("End year cannot be before starting year.");
            }

            var result = new IDataLoaderService.DataLoaderResult(0);
            try
            {
                if (useDblp)
                {
                    result += loaderService.LoadFromDblp();
                }
                if (useIeeeXplore)
                {
                    result += loaderService.LoadFromIeeeXplore();
                }
                if (useGoogleScholar)
                {
                    result += loaderService.LoadFromGoogleScholar();
                }
            }
            catch (Exception)
            {
                // TODO
                throw;  // Return 500 Internal Error
            }

            return Ok(result);
        }
    }
}