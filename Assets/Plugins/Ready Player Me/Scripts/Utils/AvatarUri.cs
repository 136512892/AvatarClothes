using System;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReadyPlayerMe
{
    public class AvatarUri
    {
        private readonly string[] Extensions = { ".glb", ".gltf" };

        private const string ShortCodeRegex = "^[A-Z0-9]{6}$";
        private const string ShortCodeUrlRegex = "^(https://readyplayer.me/api/avatar/)[A-Z0-9]{6}$";

        private const string ShortCodeBaseUrl = "https://readyplayer.me/api/avatar/";

        public string Extension { get; private set; }
        public string ModelName { get; private set; }
        public string ModelPath { get; private set; }
        public string AbsoluteUrl { get; private set;}
        public string AbsolutePath {get; private set; }
        public string AbsoluteName { get; private set; }
        public string MetaDataUrl { get; private set; }
        
        // Cover all possible cases
        // short code only, short code in url, and glb url
        public async Task<AvatarUri> Create(string url)
        {
            if (Regex.Match(url, ShortCodeRegex).Length > 0)
            {
                url = await GetUrlFromShortCode(url);
            }
            else if (Regex.Match(url, ShortCodeUrlRegex).Length > 0)
            {
                url = await GetUrlFromShortCode(url.Substring(url.Length - 6));
            }

            return CreateFromUrl(url);
        }

        private AvatarUri CreateFromUrl(string url)
        {
            Uri uri = new Uri(url);

            AbsoluteUrl = uri.AbsoluteUri;
            AbsolutePath = uri.AbsolutePath;
            AbsoluteName = Path.GetFileNameWithoutExtension(AbsolutePath);

            Extension = Path.GetExtension(AbsolutePath);
            if (!Extensions.Contains(Extension))
            {
                throw new Exception($"Exceptions.UnsupportedExtensionException: Unsupported model extension { Extension }. Only .gltf and .glb formats are supported.");
            }

            ModelName = AbsolutePath.Split('/').Last();
            ModelPath = $"{ Application.dataPath }/Resources/Avatars/{ ModelName }";

            MetaDataUrl = AbsoluteUrl.Replace(".glb", ".json");

            return this;
        }

        private static async Task<string> GetUrlFromShortCode(string shortCode)
        {
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ShortCodeBaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();

                response = await client.GetAsync(shortCode);
            }

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Exceptions.ShortCodeNotFound: Avatar at given short code { shortCode } is not found. Please make sure you entered a valid short code. HttpStatusCode: { ((int)response.StatusCode)} - { response.StatusCode }");
            }

            return response.RequestMessage.RequestUri.AbsoluteUri;
        }
    }
}
