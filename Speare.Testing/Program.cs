﻿using Speare.Compiler;
using Speare.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speare.Runtime;

namespace Speare.Testing
{
    class Program
    {
        public static void PrintVector(string x)
        {
            Console.WriteLine(x);
        }

        static OpBuilder Test1()
        {
            return new OpBuilder()

            .Method("Start")
                .Constant(Register.Local0, 0)
                .Constant(Register.Local1, 5)
                .Label(":loop")
                .Constant(Register.Local2, 1)
                .Arithmetic(Register.Local0, Register.Local2, Arithmetic.Add)
                .Set(Register.Local0, Register.LastResult)
                .DebugPrint(Register.Local0)
                .Arithmetic(Register.Local0, Register.Local1, Arithmetic.LessThan)
                .JumpIf(":loop")
                .Constant(Register.Local3, "We made it through the loop!")
                .DebugPrint(Register.Local3)
                .Constant(Register.Param0, "Another test")
                .Call(methodIndex: 1)
                .DebugPrint(Register.LastResult)
                .GlobalRead(Register.Param0, "TestVar")
                .Interop("PrintVector")
            .Method("TestMethod", parameterCount: 1)
                .GlobalRead(Register.Local0, "TestVar")
                .DebugPrint(Register.Param0)
                .Constant(Register.LastResult, 199)
                .Return();
        }

        static OpBuilder TestGlobalReadWrite()
        {
            return new OpBuilder()

            .Method("Start")
                .GlobalRead(Register.Local0, "TestVar")
                .Constant(Register.Local1, 100)
                .Arithmetic(Register.Local0, Register.Local1, Arithmetic.Multiply)
                .GlobalWrite("TestVar", Register.LastResult);
        }


        static void TokenizerTest()
        {
            var code = "{\n TestMethod(OtherMethod(), 2)\n}\nTestMethod(a, b)\n{\n}";

            var tokens = Lexer.Tokenize(code);
            foreach (var token in tokens)
            {
                if (token.Type == TokenType.EndOfFile)
                    break;

                Console.WriteLine(token.ToFormattedString());
            }

            OpBuilder ops;
            List<CompilerError> errors;

            //Compiler.Compiler.Compile(tokens, out ops, out errors);

            Console.ReadLine();
        }

        static void VMTest()
        {
            Interop.RegisterMethodsOf<Program>();

            var machine = VM.FromByteCode(Test1().Build());
            machine["TestVar"] = "From C#";
            machine.Run(methodIndex: 0);

            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            TokenizerTest();
        }
    }
}
