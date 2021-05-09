﻿using AwesomeGithubStats.Core.Interfaces;
using AwesomeGithubStats.Core.Models;
using AwesomeGithubStats.Core.Models.Svgs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using TimeSpan = System.TimeSpan;

namespace AwesomeGithubStats.Core.Services
{
    class SvgService : ISvgService
    {
        private readonly string _contentRoot;
        private readonly ICacheService _cacheService;
        private readonly RankDegree _degree;

        public SvgService(
            IWebHostEnvironment environment,
            ICacheService cacheService,
            IOptions<RankDegree> rankDegree)
        {
            _degree = rankDegree.Value;
            _contentRoot = environment.ContentRootPath;
            _cacheService = cacheService;
        }

        public async Task<Stream> GetUserStatsImage(UserRank rank, UserStatsOptions options)
        {
            var file = await GetFile("user-stats.svg");
            var svg = new UserStatsSvg(file, _degree);

            return svg.Svg(rank, new Styles());
        }

        private async Task<string> GetFile(string file)
        {
            var svgContent = _cacheService.Get<string>($"FILE:{file}");
            if (!string.IsNullOrEmpty(svgContent))
                return svgContent;

            svgContent = await File.ReadAllTextAsync(Path.Combine(_contentRoot, @"svgs\", "user-stats.svg"));

            _cacheService.Set($"FILE:{file}", svgContent, TimeSpan.FromDays(30));

            return svgContent;
        }
    }
}
