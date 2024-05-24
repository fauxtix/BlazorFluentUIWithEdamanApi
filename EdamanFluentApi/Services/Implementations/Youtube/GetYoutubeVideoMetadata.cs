﻿using EdamanFluentApi.Data;
using EdamanFluentApi.Models.Youtube.Dtos;
using EdamanFluentApi.Services.Interfaces.Youtube;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace EdamanFluentApi.Services.Implementations.Youtube
{
    public class GetYoutubeVideoMetadata : IGetYoutubeVideoMetadata
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _appName;

        public GetYoutubeVideoMetadata(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiKey = _configuration["YoutubeKeys:YouTubeApiKey"];
            _appName = _configuration["YoutubeKeys:YouTubeApplicationName"];
        }

        public async Task<YouTubeVideoDetails> GetVideoMetadata(string searchRequest_Id)
        {
            using (var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _apiKey,
                ApplicationName = _appName
            }))
            {
                var searchRequest = youtubeService.Videos.List("snippet,contentDetails");
                searchRequest.Id = searchRequest_Id;

                var searchResponse = await searchRequest.ExecuteAsync();
                var youTubeVideo = searchResponse.Items.FirstOrDefault();

                if (youTubeVideo != null)
                {
                    TimeSpan YouTubeDuration = System.Xml.XmlConvert.ToTimeSpan(youTubeVideo.ContentDetails.Duration);
                    string sDuration = YouTubeDuration.ToString();

                    YouTubeVideoDetails videoDetails = new YouTubeVideoDetails()
                    {
                        VideoId = youTubeVideo.Id,
                        Description = youTubeVideo.Snippet.Description,
                        Title = youTubeVideo.Snippet.Title,
                        ChannelTitle = youTubeVideo.Snippet.ChannelTitle,
                        PublicationDate = youTubeVideo.Snippet.PublishedAt,
                        Duration = sDuration,
                        Thumbnail = youTubeVideo.Snippet.Thumbnails.Standard is not null ?
                            youTubeVideo.Snippet.Thumbnails.Standard.Url :
                            youTubeVideo.Snippet.Thumbnails.Medium is not null ?
                            youTubeVideo.Snippet.Thumbnails.Medium.Url :
                            youTubeVideo.Snippet.Thumbnails.Maxres is not null ?
                            youTubeVideo.Snippet.Thumbnails.Maxres.Url : "Images/No-image-available.png"
                    };
                    return videoDetails;
                }

                return null;
            }
        }
        public async Task<YouTubeVideoDetails> GetAlbumArtistMetadata(string searchRequest_query)
        {
            using (var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _apiKey,
                ApplicationName = _appName
            }))
            {
                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = searchRequest_query;
                searchListRequest.MaxResults = 10;

                List<string> videos = new List<string>();
                List<string> channels = new List<string>();
                List<string> playlists = new List<string>();
                List<string> thumbnails = new List<string>();

                var searchListResponse = await searchListRequest.ExecuteAsync();

                // Add each result to the appropriate list, and then display the lists of
                // matching videos, channels, and playlists.
                foreach (var searchResult in searchListResponse.Items)
                {
                    switch (searchResult.Id.Kind)
                    {
                        case "youtube#video":
                            videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
                            break;

                        case "youtube#channel":
                            channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
                            break;

                        case "youtube#playlist":
                            playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
                            break;
                    }
                }

                var youTubeVideo = searchListResponse.Items.FirstOrDefault();
                YouTubeVideoDetails videoDetails = new YouTubeVideoDetails()
                {
                    VideoId = youTubeVideo.Id.VideoId,
                    Description = youTubeVideo.Snippet.Description,
                    Title = youTubeVideo.Snippet.Title,
                    ChannelTitle = youTubeVideo.Snippet.ChannelTitle,
                    PublicationDate = youTubeVideo.Snippet.PublishedAt,
                    Thumbnail = youTubeVideo.Snippet.Thumbnails.Standard is not null ?
                                youTubeVideo.Snippet.Thumbnails.Standard.Url :
                                youTubeVideo.Snippet.Thumbnails.Medium is not null ?
                                youTubeVideo.Snippet.Thumbnails.Medium.Url :
                                youTubeVideo.Snippet.Thumbnails.Maxres is not null ?
                                youTubeVideo.Snippet.Thumbnails.Maxres.Url : "Images/No-image-available.png"
                };

                return videoDetails;

            }
        }

        public async Task<YouTubeVideoDetails> GetSingleVideoMetadata(string searchRequest_query)
        {
            try
            {
                using (var YouTubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = _apiKey,
                    ApplicationName = _appName

                }))
                {
                    var searchListRequest = YouTubeService.Videos.List("snippet");
                    searchListRequest.Id = searchRequest_query;
                    searchListRequest.MaxResults = 1;

                    var searchListResponse = await searchListRequest.ExecuteAsync();

                    var youTubeVideo = searchListResponse.Items.FirstOrDefault(); // redundant ? (MaxResults = 1...)
                    if (youTubeVideo is not null)
                    {
                        YouTubeVideoDetails videoDetail = new YouTubeVideoDetails()
                        {
                            VideoId = youTubeVideo.Id,
                            Description = youTubeVideo.Snippet.Description,
                            Title = youTubeVideo.Snippet.Title,
                            ChannelTitle = youTubeVideo.Snippet.ChannelTitle,
                            PublicationDate = youTubeVideo.Snippet.PublishedAt,
                            Thumbnail = youTubeVideo.Snippet.Thumbnails.Standard is not null ?
                                        youTubeVideo.Snippet.Thumbnails.Standard.Url :
                                        youTubeVideo.Snippet.Thumbnails.Medium is not null ?
                                        youTubeVideo.Snippet.Thumbnails.Medium.Url :
                                        youTubeVideo.Snippet.Thumbnails.Maxres is not null ?
                                        youTubeVideo.Snippet.Thumbnails.Maxres.Url : "Images/No-image-available.png"
                        };

                        return videoDetail;
                    }
                    else
                        return null;

                }

            }
            catch
            {

                throw;
            }
        }

        public async Task<YouTubeVideoDetails> SearchByArtistAndSong(string artistName, string songName)
        {
            try
            {
                using (var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = _apiKey,
                    ApplicationName = _appName
                }))
                {
                    // Construct a search query combining artist and song name
                    string searchQuery = $"{artistName} {songName} music video";

                    var searchListRequest = youtubeService.Search.List("snippet");
                    searchListRequest.Q = searchQuery;
                    searchListRequest.MaxResults = 1;

                    var searchListResponse = await searchListRequest.ExecuteAsync();

                    var youTubeVideo = searchListResponse.Items.FirstOrDefault();

                    if (youTubeVideo is not null)
                    {
                        var videoId = youTubeVideo.Id.VideoId;
                        var duration = await GetVideoDuration(videoId);
                        YouTubeVideoDetails videoDetail = new YouTubeVideoDetails()
                        {
                            VideoId = videoId,
                            Description = youTubeVideo.Snippet.Description,
                            Title = youTubeVideo.Snippet.Title,
                            Duration = duration,
                            ChannelTitle = youTubeVideo.Snippet.ChannelTitle,
                            PublicationDate = youTubeVideo.Snippet.PublishedAt,
                            Thumbnail = youTubeVideo.Snippet.Thumbnails.Standard is not null ?
                                        youTubeVideo.Snippet.Thumbnails.Standard.Url :
                                        youTubeVideo.Snippet.Thumbnails.Medium is not null ?
                                        youTubeVideo.Snippet.Thumbnails.Medium.Url :
                                        youTubeVideo.Snippet.Thumbnails.Maxres is not null ?
                                        youTubeVideo.Snippet.Thumbnails.Maxres.Url : "Images/No-image-available.png"
                        };

                        return videoDetail;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        private async Task<string> GetVideoDuration(string searchRequest_Id)
        {
            string duration = "";
            using (var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _apiKey,
                ApplicationName = _appName
            }))
            {

                var searchRequest = youtubeService.Videos.List("snippet,contentDetails");
                searchRequest.Id = searchRequest_Id;

                var searchResponse = await searchRequest.ExecuteAsync();
                var youTubeVideo = searchResponse.Items.FirstOrDefault();

                if (youTubeVideo != null)
                {
                    TimeSpan YouTubeDuration = System.Xml.XmlConvert.ToTimeSpan(youTubeVideo.ContentDetails.Duration);
                    duration = YouTubeDuration.ToString();

                }
            }
            return duration;
        }
    }
}
