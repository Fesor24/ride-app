namespace Soloride.Infrastructure.Store;
internal sealed class S3Options
{
    public string BucketName { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string SecretAccessKey { get; init; } = string.Empty;
}
