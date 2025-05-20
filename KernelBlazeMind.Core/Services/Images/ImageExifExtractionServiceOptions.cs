namespace KernelBlazeMind.Abstraction.Services.Images
{
    public class ImageExifExtractionServiceOptions
    {
        public bool EnableExtraction { get; set; } = true;
        public int MaxMetadataLength { get; set; } = 4096;
    }
}
