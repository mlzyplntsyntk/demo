using Demo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Demo.Filters
{
    /**
     * Authorizes the users which use DataController Web API
     * The project Demo.Client demonstrates the usage of this filter
     **/
    public class ExternalAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        UserType? userType;

        public ExternalAuthorizationAttribute() { }

        public ExternalAuthorizationAttribute(UserType userType)
        {
            this.userType = userType;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.tokenKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = secretKey,
                ValidateIssuer = true,
                ValidIssuer = "Demo",
                ValidateAudience = true,
                ValidAudience = "ClientApp"
            };

            SecurityToken validatedToken;
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.InboundClaimTypeMap.Clear();
            var headers = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            try
            {
                tokenHandler.ValidateToken(headers, validationParameters, out validatedToken);
            } catch (Exception ex)
            {
                context.Result = new JsonResult(new
                {
                    error = ex.Message
                });
            }
            
        }
    }
}
