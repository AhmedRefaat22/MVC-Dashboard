using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using System;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace Demo.PL.Helpers
{
	public static class DocumentSettings
	{
		public static string UploadFile(IFormFile file , string FolderName)
		{
			// 1. Get Located Folder Path
			string FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", FolderName);

			//2. Get File Name and Make It Unique

			string FileName = $"{Guid.NewGuid()}{file.FileName}";

			//3. Get File path 

			string FilePath = Path.Combine(FolderPath, FileName);

			// 4. Save File As Streams 

			using (var fs = new FileStream(FilePath, FileMode.Create))
			file.CopyTo(fs);
			return FileName;

	
		}

		public static void DeleteFile(string FileName , string FolderName)
		{
			// 1. Get File Path
			var FilePath = Path.Combine(Directory.GetCurrentDirectory() , "wwwroot\\Files" , FolderName , FileName);

			// 2. Check is File Exsits

			if (File.Exists(FilePath))
				File.Delete(FilePath);
			
		}
	
	}
}
