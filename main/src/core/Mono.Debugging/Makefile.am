
ASSEMBLY_BUILD_DIR = $(top_builddir)/build/bin
ASSEMBLY = $(ASSEMBLY_BUILD_DIR)/Mono.Debugging.dll

SIGNING_KEY=$(srcdir)/mono.debugging.snk

DEPS = $(top_builddir)/build/bin/OldNRefactory.dll

REFS =  \
	-r:System \
	-r:System.Core \
	-r:System.Xml

FILES =  \
	Mono.Debugging.Backend/DissassemblyBuffer.cs \
	Mono.Debugging.Backend/IBacktrace.cs \
	Mono.Debugging.Backend/IDebuggerSessionFrontend.cs \
	Mono.Debugging.Backend/IObjectValueSource.cs \
	Mono.Debugging.Backend/IObjectValueUpdateCallback.cs \
	Mono.Debugging.Backend/IObjectValueUpdater.cs \
	Mono.Debugging.Backend/UpdateCallback.cs \
	Mono.Debugging.Client/AssemblyLine.cs \
	Mono.Debugging.Client/Backtrace.cs \
	Mono.Debugging.Client/BreakEvent.cs \
	Mono.Debugging.Client/BreakEventArgs.cs \
	Mono.Debugging.Client/BreakEventInfo.cs \
	Mono.Debugging.Client/BreakEventStatus.cs \
	Mono.Debugging.Client/Breakpoint.cs \
	Mono.Debugging.Client/BreakpointEventArgs.cs \
	Mono.Debugging.Client/BreakpointStore.cs \
	Mono.Debugging.Client/Catchpoint.cs \
	Mono.Debugging.Client/CatchpointEventArgs.cs \
	Mono.Debugging.Client/CompletionData.cs \
	Mono.Debugging.Client/DebuggerException.cs \
	Mono.Debugging.Client/DebuggerFeatures.cs \
	Mono.Debugging.Client/DebuggerSession.cs \
	Mono.Debugging.Client/DebuggerSessionOptions.cs \
	Mono.Debugging.Client/DebuggerStartInfo.cs \
	Mono.Debugging.Client/EvaluationOptions.cs \
	Mono.Debugging.Client/ExceptionInfo.cs \
	Mono.Debugging.Client/FunctionBreakpoint.cs \
	Mono.Debugging.Client/IExpressionEvaluator.cs \
	Mono.Debugging.Client/ObjectPath.cs \
	Mono.Debugging.Client/ObjectValue.cs \
	Mono.Debugging.Client/ObjectValueFlags.cs \
	Mono.Debugging.Client/ProcessEventArgs.cs \
	Mono.Debugging.Client/ProcessInfo.cs \
	Mono.Debugging.Client/RawValue.cs \
	Mono.Debugging.Client/SourceLocation.cs \
	Mono.Debugging.Client/StackFrame.cs \
	Mono.Debugging.Client/TargetEventArgs.cs \
	Mono.Debugging.Client/TargetEventType.cs \
	Mono.Debugging.Client/ThreadEventArgs.cs \
	Mono.Debugging.Client/ThreadInfo.cs \
	Mono.Debugging.Evaluation/ArrayElementGroup.cs \
	Mono.Debugging.Evaluation/ArrayValueReference.cs \
	Mono.Debugging.Evaluation/AsyncEvaluationTracker.cs \
	Mono.Debugging.Evaluation/AsyncOperationManager.cs \
	Mono.Debugging.Evaluation/BaseBacktrace.cs \
	Mono.Debugging.Evaluation/BaseTypeViewSource.cs \
	Mono.Debugging.Evaluation/EvaluationContext.cs \
	Mono.Debugging.Evaluation/ExceptionInfoSource.cs \
	Mono.Debugging.Evaluation/ExpressionEvaluator.cs \
	Mono.Debugging.Evaluation/FilteredMembersSource.cs \
	Mono.Debugging.Evaluation/ICollectionAdaptor.cs \
	Mono.Debugging.Evaluation/IObjectSource.cs \
	Mono.Debugging.Evaluation/IStringAdaptor.cs \
	Mono.Debugging.Evaluation/LiteralValueReference.cs \
	Mono.Debugging.Evaluation/NamespaceValueReference.cs \
	Mono.Debugging.Evaluation/NRefactoryEvaluator.cs \
	Mono.Debugging.Evaluation/NRefactoryResolverVisitor.cs \
	Mono.Debugging.Evaluation/NullValueReference.cs \
	Mono.Debugging.Evaluation/ObjectValueAdaptor.cs \
	Mono.Debugging.Evaluation/RawViewSource.cs \
	Mono.Debugging.Evaluation/RemoteFrameObject.cs \
	Mono.Debugging.Evaluation/RemoteRawValue.cs \
	Mono.Debugging.Evaluation/TimedEvaluator.cs \
	Mono.Debugging.Evaluation/TimeOutException.cs \
	Mono.Debugging.Evaluation/TypeValueReference.cs \
	Mono.Debugging.Evaluation/UserVariableReference.cs \
	Mono.Debugging.Evaluation/ValueReference.cs \
	Mono.Debugging.Evaluation/IStringAdaptor.cs

RES = 

all: $(ASSEMBLY) $(ASSEMBLY).mdb

$(ASSEMBLY): $(build_sources) $(build_resources) $(SIGNING_KEY) $(DEPS)
	mkdir -p $(ASSEMBLY_BUILD_DIR)
	$(CSC) $(CSC_FLAGS) -debug -out:$@ /keyfile:$(SIGNING_KEY) -target:library $(REFS) $(build_deps) $(build_resources:%=/resource:%) $(build_sources)

$(ASSEMBLY).mdb: $(ASSEMBLY)

assemblydir = $(MD_ASSEMBLY_DIR)
assembly_DATA = $(ASSEMBLY) $(ASSEMBLY).mdb

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb

EXTRA_DIST = $(FILES) $(RES) $(SIGNING_KEY)

include $(top_srcdir)/Makefile.include
