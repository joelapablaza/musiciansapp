using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using API.Repositories;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary; // Objeto Cloudinary que se utilizará para subir y eliminar imágenes
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account
            (
                config.Value.CloudName, // Nombre de la nube de Cloudinary
                config.Value.ApiKey, // API Key de Cloudinary
                config.Value.ApiSecret // API Secret de Cloudinary
            );

            _cloudinary = new Cloudinary(acc); // Se inicializa el objeto Cloudinary con las credenciales de la cuenta
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult(); // Objeto que guardará el resultado de la subida de la imagen

            if (file.Length > 0) // Si el tamaño del archivo es mayor a cero, se procede a subir la imagen
            {
                using var stream = file.OpenReadStream(); // Se abre un stream para leer el archivo
                var uploadParams = new ImageUploadParams // Se crean los parámetros para subir la imagen
                {
                    File = new FileDescription(file.FileName, stream), // Se especifica el nombre del archivo y el stream que lo contiene
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face") // Se aplican ciertas transformaciones a la imagen antes de subirla (en este caso, se reduce su tamaño y se enfoca en el rostro)
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams); // Se sube la imagen y se guarda el resultado en uploadResult
            }

            return uploadResult; // Se devuelve el resultado de la subida de la imagen
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId); // Se crean los parámetros para eliminar la imagen

            var result = await _cloudinary.DestroyAsync(deleteParams); // Se elimina la imagen y se guarda el resultado en result

            return result; // Se devuelve el resultado de la eliminación de la imagen
        }
    }
}