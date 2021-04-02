using System.ComponentModel.DataAnnotations;

namespace SertifiTestApi.Filters
{
    public class EmailRegularExpressionAttribute : RegularExpressionAttribute
    {
        public EmailRegularExpressionAttribute() : base(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")
        {
        }

        public override bool IsValid(object value)
        {
            ErrorMessage = $"[{value}] is not a valid email.";
            return base.IsValid(value);
        }
    }
}

