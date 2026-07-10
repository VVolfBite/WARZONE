using System.Collections.Generic;

namespace Warzone.Content.Validation
{
    public sealed class ContentValidationResult
    {
        private readonly List<ContentValidationIssue> _issues = new List<ContentValidationIssue>();

        public IReadOnlyList<ContentValidationIssue> Issues
        {
            get { return _issues; }
        }

        public bool IsValid
        {
            get { return _issues.Count == 0; }
        }

        public bool HasCriticalIssues
        {
            get
            {
                for (int i = 0; i < _issues.Count; i++)
                {
                    if (_issues[i] != null && _issues[i].Severity == ContentValidationSeverity.Error)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void Add(ContentValidationIssue issue)
        {
            if (issue != null)
            {
                _issues.Add(issue);
            }
        }
    }
}
