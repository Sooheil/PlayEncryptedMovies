using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Web.Http;
using System.Windows.Forms;

namespace WebApplication4.Controllers
{
	public class MediaController : ApiController
	{
		public const int ReadStreamBufferSize = 1024 * 1024;
		/// <summary>
		/// All types of videos
		/// </summary>
		public static readonly Dictionary<string, string> MimeNames;

		static MediaController()
		{
			if (MimeNames == null)
			{
				MimeNames = new Dictionary<string, string>();
				MimeNames.Add(".mp3", "audio/mpeg");
				MimeNames.Add(".mp4", "video/mp4");
				MimeNames.Add(".ogg", "application/ogg");
				MimeNames.Add(".ogv", "video/ogg");
				MimeNames.Add(".oga", "audio/ogg");
				MimeNames.Add(".wav", "audio/x-wav");
				MimeNames.Add(".webm", "video/webm");
			}
		}
		/// <summary>
		/// Get extension and return mime name.
		/// </summary>
		/// <param name="ext">extension</param>
		/// <returns>mime name</returns>
		private static MediaTypeHeaderValue GetMimeNameFromExtension(string ext)
		{
			string value = string.Empty;
			if (MimeNames.TryGetValue(ext.ToLowerInvariant(), out value))
				return new MediaTypeHeaderValue(value);
			else
				return new MediaTypeHeaderValue(MediaTypeNames.Application.Octet);
		}

		private static bool TryReadRangeItem(RangeItemHeaderValue range, long contentLength, out long start, out long end)
		{
			if (range.From != null)
				start = range.From.Value;
			else
				start = 0;

			if (range.To != null)
				end = range.To.Value;
			else
				end = contentLength - 1;
			return (start < contentLength && end < contentLength);
		}

		private static void CreatePartialContent(Stream inputStream, Stream outputStream, long start, long end)
		{
			int count = 0;
			long remainingBytes = end - start + 1;
			long position = start;
			byte[] buffer = new byte[ReadStreamBufferSize];
			inputStream.Position = start;
			do
			{
				try
				{
					if (remainingBytes > ReadStreamBufferSize)
						count = inputStream.Read(buffer, 0, ReadStreamBufferSize);
					else
						count = inputStream.Read(buffer, 0, (int)remainingBytes);
					outputStream.Write(buffer, 0, count);
				}
				catch (Exception error)
				{
					Debug.WriteLine(error);
					break;
				}
				position = inputStream.Position;
				remainingBytes = end - position + 1;
			} while (position <= end);
		}

		[HttpGet]
		public HttpResponseMessage Play(string file)
		{
			FileInfo fileInfo = new FileInfo(file);
			if (!fileInfo.Exists)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			MemoryStream output = WindowsPlayerSample.ContentEncryption.DecryptBinaryFile(fileInfo.FullName);

			long totalLength = fileInfo.Length;

			RangeHeaderValue rangeHeader = base.Request.Headers.Range;
			HttpResponseMessage response = new HttpResponseMessage();
			response.Headers.AcceptRanges.Add("bytes");

			if (rangeHeader == null || !rangeHeader.Ranges.Any())
			{
				response.StatusCode = HttpStatusCode.Unauthorized;
				response.Content = new StreamContent(Stream.Null);
				response.Content.Headers.ContentRange = new ContentRangeHeaderValue(totalLength);
				response.Content.Headers.ContentType = GetMimeNameFromExtension(fileInfo.Extension);
				return response;
			}

			long start = 0, end = 0;
			if (rangeHeader.Unit != "bytes" || rangeHeader.Ranges.Count > 1 || !TryReadRangeItem(rangeHeader.Ranges.First(), totalLength, out start, out end))
			{
				response.StatusCode = HttpStatusCode.Unauthorized;
				response.Content = new StreamContent(Stream.Null);
				response.Content.Headers.ContentRange = new ContentRangeHeaderValue(totalLength);
				response.Content.Headers.ContentType = GetMimeNameFromExtension(fileInfo.Extension);
				return response;
			}

			var contentRange = new ContentRangeHeaderValue(start, end, totalLength);
			response.StatusCode = HttpStatusCode.PartialContent;
			response.Content = new PushStreamContent((outputStream, httpContent, transpContext)
			=>
			{
				using (outputStream)
				using (Stream inputStream = output)
					CreatePartialContent(inputStream, outputStream, start, end);

			}, GetMimeNameFromExtension(fileInfo.Extension));

			response.Content.Headers.ContentLength = end - start + 1;
			response.Content.Headers.ContentRange = contentRange;
			return response;
		}
	}
}
