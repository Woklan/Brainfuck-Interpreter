namespace BrainfuckInterprer
{
    public sealed class BrainfuckInterpreter
    {
        private byte[] _stack = new byte[30000];
        private int _stackIndex = 0;
        private int _inputIndex = 0;
        private Stack<int> _loopIndexStack = new();

        private Dictionary<int, int> _loopPairIndexLookup = [];

        public static void Main()
            => new BrainfuckInterpreter().InterpretAsync().GetAwaiter().GetResult();

        public async Task InterpretAsync()
        {
            var fileContent = await GetFileContentAsync();

            IntializeStack();

            PopulateLoopPairIndexLookup(fileContent);

            InterpretCommands(fileContent);
        }

        private void PopulateLoopPairIndexLookup(string content)
        {
            var temporaryStack = new Stack<int>();

            var index = 0;

            foreach (var symbol in content)
            {
                if (symbol == '[')
                {
                    temporaryStack.Push(index);
                }

                if (symbol == ']')
                {
                    var startingIndex = temporaryStack.Pop();

                    _loopPairIndexLookup.Add(startingIndex, index);
                }

                index += 1;
            }

            if(temporaryStack.Count > 0)
            {
                throw new ArgumentException("Invalid brainfuck file!");
            }
        }

        private async Task<string> GetFileContentAsync()
        {
            var filePickResult  = NativeFileDialogSharp.Dialog.FileOpen();
            if (filePickResult.IsError)
            {
                throw new ArgumentException("Failed to select a folder");
            }

            var path = filePickResult.Path;

            var fileLines = await File.ReadAllLinesAsync(path) ?? throw new ArgumentException("Selected file didn't contain any information");
            
            var programOutput = string.Join(string.Empty, fileLines);
            programOutput = programOutput.Replace(Environment.NewLine, string.Empty).Trim().Replace(" ", string.Empty);

            return programOutput;
        }

        private void InterpretCommands(string input)
        {
            while (_inputIndex != input.Length)
            {
                var symbol = input[_inputIndex];

                if(symbol == '>')
                {
                    IncrementPointerIndex();
                }

                if(symbol == '<')
                {
                    DecrementPointerIndex();
                }

                if (symbol == '+')
                {
                    IncrementValueAtPointer();
                }

                if(symbol == '-')
                {
                    DecrementValueAtPointer();
                }

                if(symbol == '[')
                {
                    if (_stack[_stackIndex] == 0)
                    {
                        _inputIndex = _loopPairIndexLookup[_inputIndex];
                    }
                    else
                    {
                        _loopIndexStack.Push(_inputIndex);
                    }
                }

                if(symbol == ']')
                {
                    if(_stack[_stackIndex] != 0)
                    {
                        _inputIndex = _loopIndexStack.Peek();
                    }
                    else
                    {
                        _loopIndexStack.Pop();
                    }
                }

                if(symbol == ',')
                {
                    UpdateValueAtPointerWithUserInput();
                }

                if(symbol == '.')
                {
                    OutputValueAtPointer();
                }

                _inputIndex += 1;
            }
        }

        private void IntializeStack()
        {
            for (var i = 0; i < 30000; i++)
            {
                _stack[i] = 0;
            }
        }

        private void IncrementPointerIndex()
        {
            if (_stackIndex + 1 >= 30000)
            {
                throw new IndexOutOfRangeException();
            }

            _stackIndex += 1;
        }

        private void DecrementPointerIndex()
        {
            if (_stackIndex - 1 < 0)
            {
                throw new IndexOutOfRangeException();
            }

            _stackIndex -= 1;
        }

        private void IncrementValueAtPointer()
        {
            if (_stack[_stackIndex] == byte.MaxValue)
            {
                throw new OverflowException();
            }

            _stack[_stackIndex] = (byte)((_stack[_stackIndex] + 1) % 256);
        }

        private void DecrementValueAtPointer()
        {
            if (_stack[_stackIndex] > 0)
            {
                _stack[_stackIndex] = (byte)((_stack[_stackIndex] - 1) % 256);
            }
        }

        private void OutputValueAtPointer()
        {
            Console.Write((char)_stack[_stackIndex]);
        }

        private void UpdateValueAtPointerWithUserInput()
        {
            _stack[_stackIndex] = (byte)Console.ReadKey().KeyChar;
        }
    }
}
