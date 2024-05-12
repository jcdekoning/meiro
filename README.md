# Meiro - TvMaze Scraper

## Description

Meiro is Japanese for "maze." In this assignment for RTL, the shows from the TvMaze API are scraped and stored in local storage. Shows will be exposed through an API along with the cast.

## Prerequisites

To run this project, make sure that you have MongoDB running:

```docker run --name mongodb -d -p 27017:27017 mongo```

## Improvements

- The import shows orchestrator is just looping through the show pages and retrieves the cast for each show. This could be written with `Task.WhenAll` along with a `SemaphoreSlim` to limit the number of threads and implement the retrieval of the shows in parallel. I kept it simple since the TvMaze API rate limit is the bottleneck in this assignment.
- The import can be slow (120 requests per minute) because of the rate limit, so it can take up to 9 hours (for 70k shows). I implemented a `DynamicRateLimiter` that initially tries to perform a higher number of requests and scales down when a 429 occurs. Over time, it will try to scale up again. I would not use this for a daily import. It would be better to store the last retrieved page ID and only get the new shows, together with the update endpoints that TvMaze provides.
- When import speed is important, it would be better to build the solution so that it can run as multiple instances at the same time where each instance has its own public IP (since rate limit is IP based). You should keep track of which pages are retrieved in a central manner.
