using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWithHostedService.Services
{
    public interface IBlobQuoteClient
    {
        Task AddBlob(Stream fileBlob, string quoteId, string quoteName);
    }
    public class BlobQuoteClient:IBlobQuoteClient
    {

        public BlobQuoteClient()
        {
        }
        public async Task AddBlob(Stream fileBlob, string quoteId, string quoteName)
        {
            BlobContainerClient blobContainerClient = new BlobContainerClient("UseDevelopmentStorage=true", "quotes");
            blobContainerClient.CreateIfNotExists();
            var quoteBlob = blobContainerClient.GetBlobClient($"{quoteName}.csv");
            await quoteBlob.UploadAsync(fileBlob,true);
            
        }
    }
}
