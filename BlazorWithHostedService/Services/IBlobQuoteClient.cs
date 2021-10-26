using Azure.Storage.Blobs;
using BlazorWithHostedService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWithHostedService.Services
{
    public interface IBlobQuoteClient
    {
        Task<GetQuoteModelResponse> AddBlob(Stream fileBlob, string quoteId, string quoteName);
    }
    public class BlobQuoteClient:IBlobQuoteClient
    {

        public BlobQuoteClient()
        {
        }
        public async Task<GetQuoteModelResponse> AddBlob(Stream fileBlob, string quoteId, string quoteName)
        {
            BlobContainerClient blobContainerClient = new BlobContainerClient("UseDevelopmentStorage=true", "quotes");
            blobContainerClient.CreateIfNotExists();
            var quoteBlob = blobContainerClient.GetBlobClient($"{quoteName}.csv");
            await quoteBlob.UploadAsync(fileBlob,true);
            return new GetQuoteModelResponse { FileName=quoteBlob.Name, QuoteId=quoteId,Name=quoteName,Size=fileBlob.Length,TimeStamp=DateTime.UtcNow};


        }
    }
}
