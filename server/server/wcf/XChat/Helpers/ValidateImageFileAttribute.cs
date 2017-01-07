using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;

namespace XChat.Helpers
{
    public class ValidateImageFileAttribute : ValidationAttribute
    {
        private bool isRequired;

        public ValidateImageFileAttribute(bool isRequired = false)
        {
            this.isRequired = isRequired;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as HttpPostedFileBase;
            if (isRequired && file == null)
            {
                return new ValidationResult("Необходимо выбрать изображение");
            }

            if (file == null)
            {
                return ValidationResult.Success;
            }

            //2 mb limitation
            if (file.ContentLength > 3 * 1024 * 1024)
            {
                return new ValidationResult("Файл должен быть размером не более 3 МБ");
            }

            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                    bool formatIsCorrect = (img.RawFormat.Equals(ImageFormat.Png) || img.RawFormat.Equals(ImageFormat.Jpeg));
                    if (!formatIsCorrect)
                    {
                        return new ValidationResult("Поддерживаются только форматы JPEG и PNG");
                    }
                }
            }
            catch
            {
                return new ValidationResult("Изображение не удалось прочитать");
            }
            return ValidationResult.Success;
        }
    }
}