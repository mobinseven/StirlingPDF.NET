# StirlingPDF.NET

Official .NET SDK for interacting with the Stirling PDF API.

## Installation

```bash
dotnet add package StirlingPDF.NET
```

## Requirements

- .NET 10.0+
- A reachable Stirling PDF server
- A configured Kiota request adapter

## What this package contains

`StirlingPdfClient` is a generated Kiota client for the Stirling PDF HTTP API.

- All operations are exposed under `client.Api.V1`
- All current operations are `POST` operations
- All request bodies are sent as `MultipartBody`
- Most file-producing operations return `Stream`
- Text conversion returns `string`
- JSON-style endpoints return generated `*PostResponse` objects backed by `AdditionalData`

## Creating the client

Set the adapter base URL before constructing `StirlingPdfClient`. If you do not set one, the client defaults to `https://api.stirling.com`.

```csharp
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using StirlingPdf;

var adapter = new HttpClientRequestAdapter(new AnonymousAuthenticationProvider())
{
    BaseUrl = "https://your-stirlingpdf-instance"
};

var client = new StirlingPdfClient(adapter);
```

## Sending requests

Every generated operation currently accepts `Microsoft.Kiota.Abstractions.MultipartBody`.

```csharp
using Microsoft.Kiota.Abstractions;

await using var pdf = File.OpenRead("sample.pdf");

var body = new MultipartBody
{
    RequestAdapter = adapter
};

body.AddOrReplacePart("fileInput", "application/pdf", pdf, "sample.pdf");
```

`AddOrReplacePart` takes:

1. The field name expected by the endpoint
2. The part content type
3. The value
4. The uploaded file name

You can add file parts and scalar parts to the same body.

## Common response patterns

### File output

Most conversion and transformation endpoints return `Stream`.

```csharp
await using var result = await client.Api.V1.Convert.Img.Pdf.PostAsync(body);
await using var output = File.Create("output.pdf");
await result!.CopyToAsync(output);
```

### Text output

`client.Api.V1.Convert.Pdf.Text.PostAsync(...)` returns a `string`.

### JSON output

Analysis and a few validation-style endpoints return generated response objects whose payload is stored in `AdditionalData`.

```csharp
var response = await client.Api.V1.Analysis.PageCount.PostAsPageCountPostResponseAsync(body);

if (response?.AdditionalData?.TryGetValue("pageCount", out var pageCount) == true)
{
    Console.WriteLine(pageCount);
}
```

### No-content operations

Filter endpoints are currently generated as `Task`-returning methods with no typed response body.

## Request configuration

Every operation accepts an optional Kiota request configuration delegate for headers and middleware options.

```csharp
await client.Api.V1.Analysis.PageCount.PostAsPageCountPostResponseAsync(
    body,
    requestConfiguration =>
    {
        requestConfiguration.Headers.Add("X-API-Key", "your-api-key");
    });
```

All methods also accept an optional `CancellationToken`.

## Helpful generated features

- `client.Api` is the entry point for `/api`
- `client.Api.V1` is the entry point for `/api/v1`
- Each request builder exposes `WithUrl(...)` if you need to target a raw URL directly
- Endpoint-specific error types are thrown for mapped non-success responses

## API surface

### Top-level builder layout

```text
client
└── Api
    └── V1
        ├── Analysis
        ├── Convert
        ├── Filter
        ├── General
        ├── Misc
        ├── Pipeline
        └── Security
```

## Analysis
| Client member | Route | Primary method | Returns | Description |
| --- | --- | --- | --- | --- |
| `client.Api.V1.Analysis.AnnotationInfo` | `{baseurl}/api/v1/analysis/annotation-info` | `PostAsAnnotationInfoPostResponseAsync` | JSON object (`AdditionalData`) | Get annotation information |
| `client.Api.V1.Analysis.BasicInfo` | `{baseurl}/api/v1/analysis/basic-info` | `PostAsBasicInfoPostResponseAsync` | JSON object (`AdditionalData`) | Get basic PDF information |
| `client.Api.V1.Analysis.DocumentProperties` | `{baseurl}/api/v1/analysis/document-properties` | `PostAsDocumentPropertiesPostResponseAsync` | JSON object (`AdditionalData`) | Get PDF document properties |
| `client.Api.V1.Analysis.FontInfo` | `{baseurl}/api/v1/analysis/font-info` | `PostAsFontInfoPostResponseAsync` | JSON object (`AdditionalData`) | Get font information |
| `client.Api.V1.Analysis.FormFields` | `{baseurl}/api/v1/analysis/form-fields` | `PostAsFormFieldsPostResponseAsync` | JSON object (`AdditionalData`) | Get form field information |
| `client.Api.V1.Analysis.PageCount` | `{baseurl}/api/v1/analysis/page-count` | `PostAsPageCountPostResponseAsync` | JSON object (`AdditionalData`) | Get PDF page count |
| `client.Api.V1.Analysis.PageDimensions` | `{baseurl}/api/v1/analysis/page-dimensions` | `PostAsPageDimensionsPostResponseAsync` | JSON object (`AdditionalData`) | Get page dimensions for all pages |
| `client.Api.V1.Analysis.SecurityInfo` | `{baseurl}/api/v1/analysis/security-info` | `PostAsSecurityInfoPostResponseAsync` | JSON object (`AdditionalData`) | Get security information |

## Convert
| Client member | Route | Primary method | Returns | Description |
| --- | --- | --- | --- | --- |
| `client.Api.V1.Convert.Eml.Pdf` | `{baseurl}/api/v1/convert/eml/pdf` | `PostAsync` | Stream | Convert EML to PDF |
| `client.Api.V1.Convert.File.Pdf` | `{baseurl}/api/v1/convert/file/pdf` | `PostAsync` | Stream | Convert a file to a PDF using LibreOffice |
| `client.Api.V1.Convert.Html.Pdf` | `{baseurl}/api/v1/convert/html/pdf` | `PostAsync` | Stream | Convert an HTML or ZIP (containing HTML and CSS) to PDF |
| `client.Api.V1.Convert.Img.Pdf` | `{baseurl}/api/v1/convert/img/pdf` | `PostAsync` | Stream | Convert images to a PDF file |
| `client.Api.V1.Convert.Markdown.Pdf` | `{baseurl}/api/v1/convert/markdown/pdf` | `PostAsync` | Stream | Convert a Markdown file to PDF |
| `client.Api.V1.Convert.Pdf.CsvEscaped` | `{baseurl}/api/v1/convert/pdf/csv` | `PostAsync` | Stream | Extracts a CSV document from a PDF |
| `client.Api.V1.Convert.Pdf.Html` | `{baseurl}/api/v1/convert/pdf/html` | `PostAsync` | Stream | Convert PDF to HTML |
| `client.Api.V1.Convert.Pdf.Img` | `{baseurl}/api/v1/convert/pdf/img` | `PostAsync` | Stream | Convert PDF to image(s) |
| `client.Api.V1.Convert.Pdf.Markdown` | `{baseurl}/api/v1/convert/pdf/markdown` | `PostAsync` | Stream | Convert PDF to Markdown |
| `client.Api.V1.Convert.Pdf.Pdfa` | `{baseurl}/api/v1/convert/pdf/pdfa` | `PostAsync` | Stream | Convert a PDF to a PDF/A |
| `client.Api.V1.Convert.Pdf.Presentation` | `{baseurl}/api/v1/convert/pdf/presentation` | `PostAsync` | Stream | Convert PDF to Presentation format |
| `client.Api.V1.Convert.Pdf.Text` | `{baseurl}/api/v1/convert/pdf/text` | `PostAsync` | string | Convert PDF to Text or RTF format |
| `client.Api.V1.Convert.Pdf.Word` | `{baseurl}/api/v1/convert/pdf/word` | `PostAsync` | Stream | Convert PDF to Word document |
| `client.Api.V1.Convert.Pdf.XmlEscaped` | `{baseurl}/api/v1/convert/pdf/xml` | `PostAsync` | Stream | Convert PDF to XML |
| `client.Api.V1.Convert.Url.Pdf` | `{baseurl}/api/v1/convert/url/pdf` | `PostAsync` | Stream | Convert a URL to a PDF |

## Filter
| Client member | Route | Primary method | Returns | Description |
| --- | --- | --- | --- | --- |
| `client.Api.V1.Filter.FilterContainsImage` | `{baseurl}/api/v1/filter/filter-contains-image` | `PostAsync` | No content | Checks if a PDF contains an image |
| `client.Api.V1.Filter.FilterContainsText` | `{baseurl}/api/v1/filter/filter-contains-text` | `PostAsync` | No content | Checks if a PDF contains set text, returns true if does |
| `client.Api.V1.Filter.FilterFileSize` | `{baseurl}/api/v1/filter/filter-file-size` | `PostAsync` | No content | Checks if a PDF is a set file size |
| `client.Api.V1.Filter.FilterPageCount` | `{baseurl}/api/v1/filter/filter-page-count` | `PostAsync` | No content | Checks if a PDF is greater than, less than, or equal to a set page count |
| `client.Api.V1.Filter.FilterPageRotation` | `{baseurl}/api/v1/filter/filter-page-rotation` | `PostAsync` | No content | Checks if a PDF is of a certain rotation |
| `client.Api.V1.Filter.FilterPageSize` | `{baseurl}/api/v1/filter/filter-page-size` | `PostAsync` | No content | Checks if a PDF is of a certain size |

## General
| Client member | Route | Primary method | Returns | Description |
| --- | --- | --- | --- | --- |
| `client.Api.V1.General.BookletImposition` | `{baseurl}/api/v1/general/booklet-imposition` | `PostAsync` | Stream | Create a booklet with proper page imposition |
| `client.Api.V1.General.Crop` | `{baseurl}/api/v1/general/crop` | `PostAsync` | Stream | Crops a PDF document |
| `client.Api.V1.General.EditTableOfContents` | `{baseurl}/api/v1/general/edit-table-of-contents` | `PostAsync` | Stream | Edit Table of Contents |
| `client.Api.V1.General.ExtractBookmarks` | `{baseurl}/api/v1/general/extract-bookmarks` | `PostAsExtractBookmarksPostResponseAsync` | JSON object (`AdditionalData`) | Extract PDF Bookmarks |
| `client.Api.V1.General.MergePdfs` | `{baseurl}/api/v1/general/merge-pdfs` | `PostAsync` | Stream | Merge multiple PDF files into one |
| `client.Api.V1.General.MultiPageLayout` | `{baseurl}/api/v1/general/multi-page-layout` | `PostAsync` | Stream | Merge multiple pages of a PDF document into a single page |
| `client.Api.V1.General.OverlayPdfs` | `{baseurl}/api/v1/general/overlay-pdfs` | `PostAsync` | Stream | Overlay PDF files in various modes |
| `client.Api.V1.General.PdfToSinglePage` | `{baseurl}/api/v1/general/pdf-to-single-page` | `PostAsync` | Stream | Convert a multi-page PDF into a single long page PDF |
| `client.Api.V1.General.RearrangePages` | `{baseurl}/api/v1/general/rearrange-pages` | `PostAsync` | Stream | Rearrange pages in a PDF file |
| `client.Api.V1.General.RemoveImagePdf` | `{baseurl}/api/v1/general/remove-image-pdf` | `PostAsync` | Stream | Remove images from file to reduce the file size. |
| `client.Api.V1.General.RemovePages` | `{baseurl}/api/v1/general/remove-pages` | `PostAsync` | Stream | Remove pages from a PDF file |
| `client.Api.V1.General.RotatePdf` | `{baseurl}/api/v1/general/rotate-pdf` | `PostAsync` | Stream | Rotate a PDF file |
| `client.Api.V1.General.ScalePages` | `{baseurl}/api/v1/general/scale-pages` | `PostAsync` | Stream | Change the size of a PDF page/document |
| `client.Api.V1.General.SplitBySizeOrCount` | `{baseurl}/api/v1/general/split-by-size-or-count` | `PostAsync` | Stream | Auto split PDF pages into separate documents based on size or count |
| `client.Api.V1.General.SplitPages` | `{baseurl}/api/v1/general/split-pages` | `PostAsync` | Stream | Split a PDF file into separate documents |
| `client.Api.V1.General.SplitPdfByChapters` | `{baseurl}/api/v1/general/split-pdf-by-chapters` | `PostAsync` | Stream | Split PDFs by Chapters |
| `client.Api.V1.General.SplitPdfBySections` | `{baseurl}/api/v1/general/split-pdf-by-sections` | `PostAsync` | Stream | Split PDF pages into smaller sections |

## Misc
| Client member | Route | Primary method | Returns | Description |
| --- | --- | --- | --- | --- |
| `client.Api.V1.Misc.AddAttachments` | `{baseurl}/api/v1/misc/add-attachments` | `PostAsync` | Stream | Add attachments to PDF |
| `client.Api.V1.Misc.AddImage` | `{baseurl}/api/v1/misc/add-image` | `PostAsync` | Stream | Overlay image onto a PDF file |
| `client.Api.V1.Misc.AddPageNumbers` | `{baseurl}/api/v1/misc/add-page-numbers` | `PostAsync` | Stream | Add page numbers to a PDF document |
| `client.Api.V1.Misc.AddStamp` | `{baseurl}/api/v1/misc/add-stamp` | `PostAsync` | Stream | Add stamp to a PDF file |
| `client.Api.V1.Misc.AutoRename` | `{baseurl}/api/v1/misc/auto-rename` | `PostAsync` | Stream | Extract header from PDF file |
| `client.Api.V1.Misc.AutoSplitPdf` | `{baseurl}/api/v1/misc/auto-split-pdf` | `PostAsync` | Stream | Auto split PDF pages into separate documents |
| `client.Api.V1.Misc.CompressPdf` | `{baseurl}/api/v1/misc/compress-pdf` | `PostAsync` | Stream | Optimize PDF file |
| `client.Api.V1.Misc.DecompressPdf` | `{baseurl}/api/v1/misc/decompress-pdf` | `PostAsync` | Stream | Decompress PDF streams |
| `client.Api.V1.Misc.ExtractImageScans` | `{baseurl}/api/v1/misc/extract-image-scans` | `PostAsync` | Stream | Extract image scans from an input file |
| `client.Api.V1.Misc.ExtractImages` | `{baseurl}/api/v1/misc/extract-images` | `PostAsync` | Stream | Extract images from a PDF file |
| `client.Api.V1.Misc.Flatten` | `{baseurl}/api/v1/misc/flatten` | `PostAsync` | Stream | Flatten PDF form fields or full page |
| `client.Api.V1.Misc.OcrPdf` | `{baseurl}/api/v1/misc/ocr-pdf` | `PostAsync` | Stream | Process a PDF file with OCR |
| `client.Api.V1.Misc.RemoveBlanks` | `{baseurl}/api/v1/misc/remove-blanks` | `PostAsync` | Stream | Remove blank pages from a PDF file |
| `client.Api.V1.Misc.Repair` | `{baseurl}/api/v1/misc/repair` | `PostAsync` | Stream | Repair a PDF file |
| `client.Api.V1.Misc.ReplaceInvertPdf` | `{baseurl}/api/v1/misc/replace-invert-pdf` | `PostAsync` | Stream | Replace-Invert Color PDF |
| `client.Api.V1.Misc.ScannerEffect` | `{baseurl}/api/v1/misc/scanner-effect` | `PostAsync` | Stream | Apply scanner effect to PDF |
| `client.Api.V1.Misc.ShowJavascript` | `{baseurl}/api/v1/misc/show-javascript` | `PostAsync` | Stream | Grabs all JS from a PDF and returns a single JS file with all code |
| `client.Api.V1.Misc.UnlockPdfForms` | `{baseurl}/api/v1/misc/unlock-pdf-forms` | `PostAsync` | Stream | Remove read-only property from form fields |
| `client.Api.V1.Misc.UpdateMetadata` | `{baseurl}/api/v1/misc/update-metadata` | `PostAsync` | Stream | Update metadata of a PDF file |

## Pipeline
| Client member | Route | Primary method | Returns | Description |
| --- | --- | --- | --- | --- |
| `client.Api.V1.Pipeline.HandleData` | `{baseurl}/api/v1/pipeline/handleData` | `PostAsync` | Stream | Execute automated PDF processing pipeline |

## Security
| Client member | Route | Primary method | Returns | Description |
| --- | --- | --- | --- | --- |
| `client.Api.V1.Security.AddPassword` | `{baseurl}/api/v1/security/add-password` | `PostAsync` | Stream | Add password to a PDF file |
| `client.Api.V1.Security.AddWatermark` | `{baseurl}/api/v1/security/add-watermark` | `PostAsync` | Stream | Add watermark to a PDF file |
| `client.Api.V1.Security.AutoRedact` | `{baseurl}/api/v1/security/auto-redact` | `PostAsync` | Stream | Redacts listOfText in a PDF document |
| `client.Api.V1.Security.CertSign` | `{baseurl}/api/v1/security/cert-sign` | `PostAsync` | Stream | Sign PDF with a Digital Certificate |
| `client.Api.V1.Security.GetInfoOnPdf` | `{baseurl}/api/v1/security/get-info-on-pdf` | `PostAsGetInfoOnPdfPostResponseAsync` | JSON object (`AdditionalData`) | Inspect information returned for a PDF |
| `client.Api.V1.Security.Redact` | `{baseurl}/api/v1/security/redact` | `PostAsync` | Stream | Redacts areas and pages in a PDF document |
| `client.Api.V1.Security.RemoveCertSign` | `{baseurl}/api/v1/security/remove-cert-sign` | `PostAsync` | Stream | Remove digital signature from PDF |
| `client.Api.V1.Security.RemovePassword` | `{baseurl}/api/v1/security/remove-password` | `PostAsync` | Stream | Remove password from a PDF file |
| `client.Api.V1.Security.SanitizePdf` | `{baseurl}/api/v1/security/sanitize-pdf` | `PostAsync` | Stream | Sanitize a PDF file |
| `client.Api.V1.Security.ValidateSignature` | `{baseurl}/api/v1/security/validate-signature` | `PostAsValidateSignaturePostResponseAsync` | JSON object (`AdditionalData`) | Validate PDF Digital Signature |
