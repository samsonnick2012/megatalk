using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace XChat.Helpers
{
    public class ValidateUnitFileAttribute : RequiredAttribute
    {
        private bool isRequired;

        public ValidateUnitFileAttribute(bool isRequired = false)
        {
            this.isRequired = isRequired;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as HttpPostedFileBase;

            if (isRequired && file == null)
            {
                return new ValidationResult("Необходимо выбрать файл");
            }

            if (file == null)
            {
                return ValidationResult.Success;
            }

            if (!Path.GetExtension(file.FileName).Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
            {
                return new ValidationResult("Необходимо выбрать файл ZIP формата");
            }

            return ValidationResult.Success;
        }
    }
}