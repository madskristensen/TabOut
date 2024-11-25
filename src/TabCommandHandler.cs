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
        public string DisplayName => Vsix.Name;

        [Import]
        public ICompletionBroker _broker = null;

        public bool ExecuteCommand(TabKeyCommandArgs args, CommandExecutionContext executionContext)
        {
            if (_broker.IsCompletionActive(args.TextView))
            {
                return false;
            }

            var moveCount = 0;
            ITextSnapshotLine line = args.TextView.Caret.Position.BufferPosition.GetContainingLine();
            var linePosition = args.TextView.Caret.Position.BufferPosition.Position - line.Start;
            var lineText = line.GetText();
            var textBeforeCaret = lineText.Substring(0, linePosition);

            if (linePosition < lineText.Length && !string.IsNullOrWhiteSpace(textBeforeCaret))
            {
                var caretChar = lineText[linePosition];
                var mc = 1;

                while (caretChar == ' ')
                {
                    mc++;
                    linePosition++;

                    if (linePosition >= lineText.Length)
                    {
                        break;
                    }

                    caretChar = lineText[linePosition];
                }

                if (caretChar is '{' or '[' or '(' or ')' or ']' or '}' or '"' or '\'' or '.' or ';' or ',')
                {
                    moveCount = mc;
                }
            }

            if (moveCount > 0)
            {
                _ = args.TextView.Caret.MoveTo(args.TextView.Caret.Position.BufferPosition + moveCount);
            }

            return moveCount > 0;
        }

        public CommandState GetCommandState(TabKeyCommandArgs args)
        {
            return CommandState.Available;
        }
    }
}
