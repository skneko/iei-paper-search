﻿using IEIPaperSearch.DataExtractors;
using IEIPaperSearch.DataExtractors.BDLP;
using IEIPaperSearch.DataExtractors.Bibtex;
using IEIPaperSearch.DataExtractors.IeeeXplore;
using IEIPaperSearch.DataSourceWrappers.DBLP;
using IEIPaperSearch.DataSourceWrappers.GoogleScholar;
using IEIPaperSearch.DataSourceWrappers.IeeeXplore;
using IEIPaperSearch.Persistence;
using OpenQA.Selenium.Edge;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static IEIPaperSearch.Services.DataLoaders.IDataLoaderService;

namespace IEIPaperSearch.Services.DataLoaders
{
    internal class DataLoaderService : IDataLoaderService
    {
        private readonly PaperSearchContext context;

        const int IEEE_XPLORE_LIMIT_PER_TYPE = 1000;
        const int GSCHLOAR_PAGE_LIMIT = 10;

        public DataLoaderService(PaperSearchContext context)
        {
            this.context = context;
        }

        public DataLoaderResult LoadFromDblp()
        {
            Console.WriteLine("Started DBLP data extraction...");

            var path = Environment.GetEnvironmentVariable("IEI_DBLP_FILE_PATH");
            if (path is null)
            {
                throw new InvalidOperationException("Can't find DBLP data: environment variable IEI_DBLP_FILE_PATH not set.");
            }
            var xml = File.ReadAllText(path);

            Console.WriteLine("Inserting DBLP data into database...");
            var count = ExtractFromJsonSource(
                new DblpDataExtractor(context, "#text"),
                new DblpXmlConverterWrapper().ExtractFromXml(xml));

            Console.WriteLine($"done extracting DBLP data ({count} entries).");

            return new DataLoaderResult(count);
        }

        public DataLoaderResult LoadFromIeeeXplore()
        {
            Console.WriteLine("Started IEEE Xplore data extraction...");

            var wrapper = new IeeeXploreApiWrapper();

            Console.WriteLine("Inserting IEEE Xplore articles data into database (1/3)...");
            var articles = wrapper.ExtractFromApi(IEEE_XPLORE_LIMIT_PER_TYPE, IeeeXploreSubmissionKind.Articles).Result;
            ExtractFromJsonSource(new IeeeXploreDataExtractor(context), articles);

            Console.WriteLine("Inserting IEEE Xplore books data into database (2/3)...");
            var books = wrapper.ExtractFromApi(IEEE_XPLORE_LIMIT_PER_TYPE, IeeeXploreSubmissionKind.Books).Result;
            var count = ExtractFromJsonSource(new IeeeXploreDataExtractor(context), books);

            Console.WriteLine("Inserting IEEE Xplore inproceedings data into database (3/3)...");
            var inProceedings = wrapper.ExtractFromApi(IEEE_XPLORE_LIMIT_PER_TYPE, IeeeXploreSubmissionKind.InProceedings).Result;
            ExtractFromJsonSource(new IeeeXploreDataExtractor(context), inProceedings);

            Console.WriteLine($"done extracting IEEE Xplore data ({count} entries).");

            return new DataLoaderResult(count);
        }

        public DataLoaderResult LoadFromGoogleScholar(string query)
        {
            Console.WriteLine("Started Google Scholar data extraction...");

            ICollection<GoogleScholarSeleniumScrapper.ScrapperResult> scrapped;
            using (var webDriver = new EdgeDriver())
            using (var scrapper = new GoogleScholarSeleniumScrapper(webDriver, GSCHLOAR_PAGE_LIMIT))
            {
                scrapped = scrapper.Scrap(query);
            }

            Console.WriteLine("Coverting Google Scholar Bibtex data to JSON...");
            var bibtex = scrapped
                    .Where(e => e.Text is not null)
                    .Select(e => e.Text!);
            var json = new BibtexJsonConverter().BibtexToJson(bibtex);

            Console.WriteLine("Inserting Google Scholar data into database...");
            var count = ExtractFromJsonSource(new BibtexDataExtractor(context), json);

            Console.WriteLine($"done extracting Google Scholar data ({count} entries).");

            return new DataLoaderResult(count);
        }

        private int ExtractFromJsonSource(IJsonDataExtractor<SubmissionDataExtractorResult> extractor, string json)
        {
            var data = extractor.Extract(json);
            var count = 0;
            
            context.Books.AddRange(data.Books);
            count += data.Books.Count;
            
            context.Articles.AddRange(data.Articles);
            count += data.Articles.Count;
            
            context.InProceedings.AddRange(data.InProceedings);
            count += data.InProceedings.Count;

            context.SaveChanges();
            return count;
        }
    }
}
