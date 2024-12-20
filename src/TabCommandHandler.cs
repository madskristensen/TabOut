using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;

namespace TabOut
{
    [Export(typeof(ICommandHandler))]
    [Name(nameof(TabCommandHandler))]
    [Order(Before = DefaultOrderings.Lowest)]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal class TabCommandHandler : ICommandHandler<TabKeyCommandArgs>
    {
        private static readonly RatingPrompt _rating = new("MadsKristensen.TabOut", Vsix.Name, General.Instance);

        public string DisplayName => Vsix.Name;

        [Import]
        public ICompletionBroker _broker = null;

        public bool ExecuteCommand(TabKeyCommandArgs args, CommandExecutionContext executionContext)
        {
            if (_broker.IsCompletionActive(args.TextView))
            {
                return false;
            }

            ITextSnapshotLine line = args.TextView.Caret.Position.BufferPosition.GetContainingLine();
            var linePosition = args.TextView.Caret.Position.BufferPosition.Position - line.Start;
            var lineText = line.GetText();
            var textBeforeCaret = lineText.Substring(0, linePosition);

            if (linePosition < lineText.Length && !string.IsNullOrWhiteSpace(textBeforeCaret))
            {
                var moveCount = 1;
                var caretChar = lineText[linePosition];

                while (caretChar == ' ')
                {
                    moveCount++;
                    linePosition++;

                    if (linePosition >= lineText.Length)
                    {
                        break;
                    }

                    caretChar = lineText[linePosition];
                }

                if (caretChar is '{' or '[' or '(' or ')' or ']' or '}' or '"' or '\'' or '.' or ';' or ',')
                {
                    _ = args.TextView.Caret.MoveTo(args.TextView.Caret.Position.BufferPosition + moveCount);
                    _rating.RegisterSuccessfulUsage();
                    return true;
                }
            }

            return false;
        }

        public CommandState GetCommandState(TabKeyCommandArgs args)
        {
            return CommandState.Available;
        }
    }
}
