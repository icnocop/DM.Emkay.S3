﻿using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DM.Emkay.S3
{
    public abstract class S3Base : Task, IDisposable
    {
        private readonly IS3ClientFactory _s3ClientFactory;
        private ITaskLogger _logger;
        private IS3Client _client;

        public const int DefaultRequestTimeout = 300000; // 5 min default timeout
        public const int DefaultBufferSizeKilobytes = 8192; // 8KB default buffer size

        public const string DefaultRegion = "us-east-1";

        protected S3Base(IS3ClientFactory s3ClientFactory,
                         int timeoutMilliseconds = DefaultRequestTimeout,
                         int bufferSizeKilobytes = DefaultBufferSizeKilobytes,
                         ITaskLogger logger = null)
        {
            _s3ClientFactory = s3ClientFactory;
            _logger = logger;
            TimeoutMilliseconds = timeoutMilliseconds;
            BufferSizeKilobytes = bufferSizeKilobytes;
        }

        public ITaskLogger Logger
        {
            get { return _logger ?? (_logger = new MsBuildTaskLogger(Log)); }
            set { _logger = value; }
        }

        [Required]
        public string Key { get; set; }

        [Required]
        public string Secret { get; set; }

        [Required]
        public string Bucket { get; set; }

        public string Region { get; set; }
        
        public int TimeoutMilliseconds { get; set; }

        public int BufferSizeKilobytes { get; set; }

        public IS3Client Client
        {
            get { return _client ?? (_client = _s3ClientFactory.Create(Key, Secret, Region ?? DefaultRegion, BufferSizeKilobytes)); }
        }

        public void Dispose()
        {
            if (null != _client)
                _client.Dispose();
            _client = null;
        }
    }
}