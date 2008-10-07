// This file is used by Code Analysis to maintain 
// CA_GLOBAL_SUPPRESS_MESSAGE macros that are applied to this project.
// Project-level suppressions either have no target or are given a
// specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in
// the Error List, point to "Suppress Message(s)", and click "In Project
// Suppression File". You do not need to add suppressions to
// this file manually.

CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope="namespace", Target="ManagedCppException");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Cpp", Scope="namespace", Target="ManagedCppException");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Cpp");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Design", "CA1014:MarkAssembliesWithClsCompliant");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Scope="type", Target="ManagedCppException.ThrowAnException");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope="member", Target="ManagedCppException.ThrowAnException.#ThrowBoxedException()");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Scope="member", Target="ManagedCppException.ThrowAnException.#ThrowManagedException()");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope="member", Target="ManagedCppException.ThrowAnException.#ThrowManagedException()");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope="member", Target="ManagedCppException.ThrowAnException.#ThrowUnmanagedException()");
