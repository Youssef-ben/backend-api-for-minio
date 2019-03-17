using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

namespace Backend.API.Config
{
    public static class LocalizationConfig
    {
        public static void Configure(IApplicationBuilder app)
        {
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en"),
                new CultureInfo("en-CA"),
                new CultureInfo("fr"),
                new CultureInfo("fr-CA"),
            };

            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                DefaultRequestCulture = new RequestCulture("en"),

                // Used to get the Language from the request header - {Accept-Language}.
                RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new AcceptLanguageHeaderRequestCultureProvider(),
                },
            };

            app.UseRequestLocalization(requestLocalizationOptions);
        }
    }
}
