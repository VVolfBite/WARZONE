namespace Warzone.Content.Validation
{
    public sealed class ContentValidationIssue
    {
        public ContentValidationIssue(string code, string message, string definitionId = null, ContentValidationSeverity severity = ContentValidationSeverity.Error)
        {
            Code = code;
            Message = message;
            DefinitionId = definitionId;
            Severity = severity;
        }

        public string Code { get; private set; }
        public string Message { get; private set; }
        public string DefinitionId { get; private set; }
        public ContentValidationSeverity Severity { get; private set; }
    }
}
