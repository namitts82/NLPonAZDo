using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ChatWithAzDoBoard.Helper
{

    public class ResponseCard
    {
        public string total { get; set; }
        public string summary { get; set; }
        public List<Workitem> workitem { get; set; }
    }

    public class Workitem
    {
        public string id { get; set; }
        public string name { get; set; }
        public string link { get; set; }
        public string itemsummary { get; set; }
    }

    public class State
    {
        public SearchResponse EngagementSummaries;
    }

    public class SerchRequest
    {
        public SerchRequest(string searchText) {
            this.searchText = searchText;
        }

        public string searchText { get; set; }

        [JsonProperty("$skip")]
        public int skip { get; set; } = 0; 

        [JsonProperty("$top")]
        public int top { get; set; } = 10;

        [JsonProperty("$orderBy")]
        public bool includeFacets { get; set; } = false;
    }

    public class Facets
    {
    }

    public class Fields
    {
        [JsonProperty("system.id")]
        public string systemid { get; set; }

        [JsonProperty("system.workitemtype")]
        public string systemworkitemtype { get; set; }

        [JsonProperty("system.title")]
        public string systemtitle { get; set; }

        [JsonProperty("system.assignedto")]
        public string systemassignedto { get; set; }

        [JsonProperty("system.state")]
        public string systemstate { get; set; }

        [JsonProperty("system.tags")]
        public string systemtags { get; set; }

        [JsonProperty("system.rev")]
        public string systemrev { get; set; }

        [JsonProperty("system.createddate")]
        public DateTime systemcreateddate { get; set; }

        [JsonProperty("system.changeddate")]
        public DateTime systemchangeddate { get; set; }
    }

    public class Hit
    {
        public string fieldReferenceName { get; set; }
        public List<string> highlights { get; set; }
    }

    public class Project
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class Result
    {
        public Project project { get; set; }
        public Fields fields { get; set; }
        public List<Hit> hits { get; set; }
        public string url { get; set; }
        public string summary { get; set; }
    }

    public class SearchResponse
    {
        public int count { get; set; }
        public List<Result> results { get; set; }
        public int infoCode { get; set; }
        public Facets facets { get; set; }
    }
}
