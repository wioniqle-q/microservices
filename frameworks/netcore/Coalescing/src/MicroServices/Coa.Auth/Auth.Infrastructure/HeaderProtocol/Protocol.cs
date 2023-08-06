using Microsoft.AspNetCore.Builder;

namespace Auth.Infrastructure.HeaderProtocol;

public static class Protocol
{
    public static void UseHeaderProtocol(this IApplicationBuilder app)
    {
        app.UseSecurityHeaders(ProtocolExtensions.HeaderPolicyCollection());
    }
}

internal static class ProtocolExtensions
{
    public static HeaderPolicyCollection HeaderPolicyCollection()
    {
        var collection = new HeaderPolicyCollection()
            .AddFrameOptionsDeny()
            .AddXssProtectionBlock()
            .AddContentTypeOptionsNoSniff()
            .AddReferrerPolicyNoReferrer()
            .AddStrictTransportSecurityMaxAge()
            .AddReferrerPolicyOriginWhenCrossOrigin()
            .RemoveServerHeader()
            .AddCrossOriginOpenerPolicy(builder => builder.SameOrigin())
            .AddCrossOriginEmbedderPolicy(builder => builder.RequireCorp())
            .AddCrossOriginResourcePolicy(builder => builder.SameOrigin())
            .AddXssProtectionEnabled()
            .AddContentSecurityPolicy(options =>
            {
                options.AddObjectSrc().None();
                options.AddBlockAllMixedContent();
                options.AddImgSrc().Self().From("data:");
                options.AddFormAction().Self();
                options.AddFontSrc().Self();
                options.AddStyleSrc().Self().UnsafeInline();
                options.AddScriptSrc().Self().UnsafeInline();
                options.AddBaseUri().Self();
                options.AddFrameAncestors().None();
                options.AddUpgradeInsecureRequests();
            })
            .AddPermissionsPolicy(options =>
            {
                options.AddAccelerometer().None();
                options.AddAmbientLightSensor().None();
                options.AddAutoplay().None();
                options.AddCamera().None();
                options.AddEncryptedMedia().None();
                options.AddFullscreen().All();
                options.AddGeolocation().None();
                options.AddGyroscope().None();
                options.AddMagnetometer().None();
                options.AddMicrophone().None();
                options.AddMidi().None();
                options.AddPayment().None();
                options.AddPictureInPicture().None();
                options.AddSpeaker().None();
                options.AddSyncXHR().None();
                options.AddUsb().None();
                options.AddVR().None();
            })
            .AddFeaturePolicy(builder =>
            {
                builder.AddAccelerometer().None();
                builder.AddAmbientLightSensor().None();
                builder.AddAutoplay().None();
                builder.AddCamera().None();
                builder.AddEncryptedMedia().None();
                builder.AddFullscreen().All();
                builder.AddGeolocation().None();
                builder.AddMidi().None();
                builder.AddPayment().None();
                builder.AddPictureInPicture().None();
                builder.AddSpeaker().None();
                builder.AddSyncXHR().None();
            });

        return collection;
    }
}