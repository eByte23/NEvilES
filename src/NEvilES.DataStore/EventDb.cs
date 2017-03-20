﻿using System;

namespace NEvilES.DataStore
{
    public class EventDb
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public Guid StreamId { get; set; }
        public Guid TransactionId { get; set; }
        public Guid Who { get; set; }
        public string AppVersion { get; set; }
        public string BodyType { get; set; }
        public string Body { get; set; }
        public DateTime When { get; set; }
        public int Version { get; set; }
        
        //Do we really need this?
        public string Metadata { get; set; }
    }
}
