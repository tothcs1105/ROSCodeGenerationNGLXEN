﻿namespace Rosbridge.CodeGeneration.Logic.UnitTests.TemplateUnitTests
{
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.VisualStudio.TextTemplating;
    using NUnit.Framework;
    using Rosbridge.CodeGeneration.Logic.Constants;
    using Rosbridge.CodeGeneration.Logic.UnitTests.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using T4Template.Utilities.Extensions;
    using T4Template.Utilities.Interfaces;
    using T4Template.Utilities.TemplateCompile;
    using T4Template.Utilities.TemplateProcess;

    [TestFixture]
    public class TimeDataTemplateUnitTests
    {
        private const string TEMPLATE_RELATIVE_PATH_TO_EXECUTING_PROJECT_DIRECTORY = @"..\Rosbridge.CodeGeneration.Logic\Templates\CodeGenerators\TimeData.tt";
        private readonly KeyValuePair<string, Type> EQUALS_METHOD = new KeyValuePair<string, Type>("Equals", typeof(Boolean));
        private readonly KeyValuePair<string, Type> SEC_PROPERTY = new KeyValuePair<string, Type>("sec", typeof(UInt32));
        private readonly KeyValuePair<string, Type> NSEC_PROPERTY = new KeyValuePair<string, Type>("nsec", typeof(UInt32));

        private static readonly IEnumerable<string> DefaultNamespaces =
        new[]
        {
                    "System",
                    "System.Linq",
        };
        private static readonly CSharpCompilationOptions DefaultCompilationOptions =
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOverflowChecks(true)
                .WithPlatform(Platform.X86)
                .WithUsings(DefaultNamespaces);
        private static readonly IEnumerable<MetadataReference> DefaultReferences =
        new[]
        {
                    MetadataReference.CreateFromFile(typeof (object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof (System.Linq.Enumerable).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof (System.GenericUriParser).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).Assembly.Location)
        };

        private FileInfo _template;
        private ICustomTextTemplatingEngineHost _textTemplatingEngineHost;
        private ITemplateProcessor _templateProcessor;
        private ITemplateCompiler _templateCompiler;

        [SetUp]
        public void SetUp()
        {
            DirectoryInfo currentProjectDirectory = ExecutingProjectFinder.GetExecutingProjectDirectory();

            if (!currentProjectDirectory.Exists)
            {
                throw new DirectoryNotFoundException(currentProjectDirectory.FullName);
            }

            _template = new FileInfo(Path.Combine(currentProjectDirectory.FullName, TEMPLATE_RELATIVE_PATH_TO_EXECUTING_PROJECT_DIRECTORY));

            if (!_template.Exists)
            {
                throw new FileNotFoundException(_template.FullName);
            }

            _textTemplatingEngineHost = new CustomTextTemplatingEngineHost(AppDomain.CurrentDomain);
            _templateProcessor = new TemplateProcessor(_textTemplatingEngineHost);
            _templateCompiler = new TemplateCompiler();
        }

        [Test]
        public void TimeDataTemplate_UnitTest_ParametersOK_TemplateCreatesAppropriateOutput()
        {
            //arrange
            string testNamespace = "testNamespace";
            string testType = "testType";
            ITextTemplatingSession session = CreateTemplateSession(testNamespace, testType);

            //act
            string templateOutput = _templateProcessor.ProcessTemplateWithSession(_template, session);

            //assert
            SyntaxTree parsedTemplateOutput = _templateCompiler.ParseTemplateOutput(templateOutput);
            Assembly compiledAssembly = _templateCompiler.CompileSyntaxTree(parsedTemplateOutput, DefaultCompilationOptions, DefaultReferences, MethodBase.GetCurrentMethod().Name);
            compiledAssembly.Should().NotBeNull();
            compiledAssembly.DefinedTypes.Should().NotBeNull();
            compiledAssembly.DefinedTypes.Should().HaveCount(1);
            Type resultType = compiledAssembly.DefinedTypes.First();
            resultType.Namespace.Should().Be(testNamespace);
            resultType.Name.Should().Be(testType);
            resultType.IsValueType.Should().BeTrue();
            MemberInfo[] memberList = resultType.GetMembers();
            MethodInfo equalsMethod = (MethodInfo)memberList.FirstOrDefault(member => member.Name == EQUALS_METHOD.Key && member.MemberType == MemberTypes.Method);
            equalsMethod.Should().NotBeNull();
            equalsMethod.ReturnType.Should().Be(EQUALS_METHOD.Value);
            PropertyInfo[] propertyList = resultType.GetProperties();
            propertyList.Should().HaveCount(2);
            propertyList.Should().Contain(property => property.Name == SEC_PROPERTY.Key && property.PropertyType == SEC_PROPERTY.Value);
            propertyList.Should().Contain(property => property.Name == NSEC_PROPERTY.Key && property.PropertyType == NSEC_PROPERTY.Value);
        }

        [Test]
        public void TimeDataTemplate_UnitTest_NamespaceParameterNull_TemplateShouldThrowNullException()
        {
            //arrange
            string testNamespace = null;
            string testType = "testType";
            ITextTemplatingSession session = CreateTemplateSession(testNamespace, testType);

            //act
            string templateOutput = _templateProcessor.ProcessTemplateWithSession(_template, session);

            //assert
            _textTemplatingEngineHost.Errors.Any(error => error.FileName == _template.FullName && error.ErrorText.Contains(typeof(NullReferenceException).Name)).Should().BeTrue();
        }

        [Test]
        public void TimeDataTemplate_UnitTest_NamespaceParameterEmpty_TemplateShouldThrowArgumentException()
        {
            //arrange
            string testNamespace = "";
            string testType = "testType";
            ITextTemplatingSession session = CreateTemplateSession(testNamespace, testType);

            //act
            string templateOutput = _templateProcessor.ProcessTemplateWithSession(_template, session);

            //assert
            _textTemplatingEngineHost.Errors.Any(error => error.FileName == _template.FullName && error.ErrorText.Contains(typeof(ArgumentException).Name)).Should().BeTrue();
        }

        [Test]
        public void TimeDataTemplate_UnitTest_TypeParameterNull_TemplateShouldThrowNullException()
        {
            //arrange
            string testNamespace = "testNamespace";
            string testType = null;
            ITextTemplatingSession session = CreateTemplateSession(testNamespace, testType);

            //act
            string templateOutput = _templateProcessor.ProcessTemplateWithSession(_template, session);

            //assert
            _textTemplatingEngineHost.Errors.Any(error => error.FileName == _template.FullName && error.ErrorText.Contains(typeof(NullReferenceException).Name)).Should().BeTrue();
        }

        [Test]
        public void TimeDataTemplate_UnitTest_TypeParameterEmpty_TemplateShouldThrowArgumentException()
        {
            //arrange
            string testNamespace = "testNamespace";
            string testType = "";
            ITextTemplatingSession session = CreateTemplateSession(testNamespace, testType);

            //act
            string templateOutput = _templateProcessor.ProcessTemplateWithSession(_template, session);

            //assert
            _textTemplatingEngineHost.Errors.Any(error => error.FileName == _template.FullName && error.ErrorText.Contains(typeof(ArgumentException).Name)).Should().BeTrue();
        }

        private ITextTemplatingSession CreateTemplateSession(string @namespace, string type)
        {
            ITextTemplatingSession session = new TextTemplatingSession();

            session[TemplateParameterConstants.TimeData.NAMESPACE] = @namespace;
            session[TemplateParameterConstants.TimeData.TYPE] = type;

            return session;
        }
    }
}
