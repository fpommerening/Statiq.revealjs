using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FP.Statiq.RevealJS.Properties;
using Statiq.Common;

namespace FP.Statiq.RevealJS.Business
{
    public class EncryptContent : Module
    {
        private const int Iterations = 1000;
        private const int KeySize = 32;

        protected override async Task<IEnumerable<IDocument>> ExecuteContextAsync(IExecutionContext context)
        {
            var input = context.Inputs.First();
            var password = input[MetadataKeys.SlideDeskPassword]?.ToString();
            if (string.IsNullOrEmpty(password))
            {
                return context.Inputs;
            }

            var salt = new byte[32];
            var iv = new byte[16];
            var key = new byte[KeySize];
            var random = new Random();
            random.NextBytes(salt);
            random.NextBytes(iv);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt))
            {
                pbkdf2.IterationCount = Iterations;
                key = pbkdf2.GetBytes(KeySize);
            }
           
            var reader = input.ContentProvider.GetTextReader();
            var planeText = await reader.ReadToEndAsync();
           
            var encryptedData = new EncryptedData
            {
                salt = Convert.ToBase64String(salt),
                iv = Convert.ToBase64String(iv)
            };

            using (var aes = new RijndaelManaged())
            {
                aes.Padding = PaddingMode.Zeros;
                var encryptor = aes.CreateEncryptor(key, iv);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(planeText);
                        }
                        var data = msEncrypt.ToArray();
                        encryptedData.data = Convert.ToBase64String(data);
                    }
                }
                
            }

            var json = JsonSerializer.Serialize(encryptedData);
            var content = Resources.pagecrypt_template
                .Replace("{{TITLE}}", $"{input[MetadataKeys.SlideDeskTitle]}")
                .Replace("/*{{ENCRYPTED_PAYLOAD}}*/\"\"", json);
            var encryptedDoc = new Document(input.Destination, context.GetContentProvider(content, MediaTypes.Html));
            return encryptedDoc.Yield();
        }


        private class EncryptedData
        {
            public string data { get; set; }
            public string iv { get; set; }
            public string salt { get; set; }
        }



    }
}
