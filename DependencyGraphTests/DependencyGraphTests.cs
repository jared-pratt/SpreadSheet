///<<authors>>
/// Written by Jared Pratt (& Joe Zachary)
///<<authors>>
///
///<<version>>
/// September 13, 2024
///<<version>>

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CS3500.DependencyGraph;
using System.Collections.Generic;

namespace CS3500.DependencyGraph
{
        /// <summary>
        ///   This is a test class for DependencyGraphTest and is intended
        ///   to contain all DependencyGraphTest Unit Tests
        /// </summary>
        [TestClass]
        public class DependencyGraphTests
        {
            private DependencyGraph graph;

            [TestInitialize]
            public void Setup()
            {
                graph = new DependencyGraph();
            }
            /// <summary>
            ///   TODO:  Explain carefully what this code tests.
            ///          Also, update in-line comments as appropriate.
            /// </summary>
            [TestMethod]
            [Timeout(2000)]  // 2 second run time limit
            public void StressTest()
            {
                DependencyGraph dg = new();

                // A bunch of strings to use
                const int SIZE = 200;
                string[] letters = new string[SIZE];
                for (int i = 0; i < SIZE; i++)
                {
                    letters[i] = string.Empty + ((char)('a' + i));
                }

                // The correct answers
                HashSet<string>[] dependents = new HashSet<string>[SIZE];
                HashSet<string>[] dependees = new HashSet<string>[SIZE];
                for (int i = 0; i < SIZE; i++)
                {
                    dependents[i] = [];
                    dependees[i] = [];
                }

                // Add a bunch of dependencies
                for (int i = 0; i < SIZE; i++)
                {
                    for (int j = i + 1; j < SIZE; j++)
                    {
                        dg.AddDependency(letters[i], letters[j]);
                        dependents[i].Add(letters[j]);
                        dependees[j].Add(letters[i]);
                    }
                }

                // Remove a bunch of dependencies
                for (int i = 0; i < SIZE; i++)
                {
                    for (int j = i + 4; j < SIZE; j += 4)
                    {
                        dg.RemoveDependency(letters[i], letters[j]);
                        dependents[i].Remove(letters[j]);
                        dependees[j].Remove(letters[i]);
                    }
                }

                // Add some back
                for (int i = 0; i < SIZE; i++)
                {
                    for (int j = i + 1; j < SIZE; j += 2)
                    {
                        dg.AddDependency(letters[i], letters[j]);
                        dependents[i].Add(letters[j]);
                        dependees[j].Add(letters[i]);
                    }
                }

                // Remove some more
                for (int i = 0; i < SIZE; i += 2)
                {
                    for (int j = i + 3; j < SIZE; j += 3)
                    {
                        dg.RemoveDependency(letters[i], letters[j]);
                        dependents[i].Remove(letters[j]);
                        dependees[j].Remove(letters[i]);
                    }
                }

                // Make sure everything is right
                for (int i = 0; i < SIZE; i++)
                {
                    Assert.IsTrue(dependents[i].SetEquals(new HashSet<string>(dg.GetDependents(letters[i]))));
                    Assert.IsTrue(dependees[i].SetEquals(new HashSet<string>(dg.GetDependees(letters[i]))));
                }
            }

            // --- The additional unit tests for each method start here ---

            // Tests for Size Property
            [TestMethod]
        public void TestSize_EmptyGraph()
        {
            Assert.AreEqual(0, graph.Size);
        }

        [TestMethod]
        public void TestSize_AddSingleDependency()
        {
            graph.AddDependency("A", "B");
            Assert.AreEqual(1, graph.Size);
        }

        [TestMethod]
        public void TestSize_AddMultipleDependencies()
        {
            graph.AddDependency("A", "B");
            graph.AddDependency("B", "C");
            Assert.AreEqual(2, graph.Size);
        }

        [TestMethod]
        public void TestSize_DuplicateDependency()
        {
            graph.AddDependency("A", "B");
            graph.AddDependency("A", "B");  // Duplicate
            Assert.AreEqual(1, graph.Size); // No duplicate should be added
        }

        [TestMethod]
        public void TestSize_RemoveDependency()
        {
            graph.AddDependency("A", "B");
            graph.RemoveDependency("A", "B");
            Assert.AreEqual(0, graph.Size);
        }

        // Tests for HasDependents Method
        [TestMethod]
        public void TestHasDependents_NodeWithDependents()
        {
            graph.AddDependency("A", "B");
            Assert.IsTrue(graph.HasDependents("A"));
        }

        [TestMethod]
        public void TestHasDependents_NodeWithoutDependents()
        {
            graph.AddDependency("A", "B");
            Assert.IsFalse(graph.HasDependents("B"));
        }

        [TestMethod]
        public void TestHasDependents_NodeDoesNotExist()
        {
            Assert.IsFalse(graph.HasDependents("NonExistentNode"));
        }

        [TestMethod]
        public void TestHasDependents_EmptyGraph()
        {
            Assert.IsFalse(graph.HasDependents("A"));
        }

        [TestMethod]
        public void TestHasDependents_AfterRemovingDependency()
        {
            graph.AddDependency("A", "B");
            graph.RemoveDependency("A", "B");
            Assert.IsFalse(graph.HasDependents("A"));
        }

        // Tests for HasDependees Method
        [TestMethod]
        public void TestHasDependees_NodeWithDependees()
        {
            graph.AddDependency("A", "B");
            Assert.IsTrue(graph.HasDependees("B"));
        }

        [TestMethod]
        public void TestHasDependees_NodeWithoutDependees()
        {
            graph.AddDependency("A", "B");
            Assert.IsFalse(graph.HasDependees("A"));
        }

        [TestMethod]
        public void TestHasDependees_NodeDoesNotExist()
        {
            Assert.IsFalse(graph.HasDependees("NonExistentNode"));
        }

        [TestMethod]
        public void TestHasDependees_EmptyGraph()
        {
            Assert.IsFalse(graph.HasDependees("A"));
        }

        [TestMethod]
        public void TestHasDependees_AfterRemovingDependency()
        {
            graph.AddDependency("A", "B");
            graph.RemoveDependency("A", "B");
            Assert.IsFalse(graph.HasDependees("B"));
        }

        // Tests for GetDependents Method
        [TestMethod]
        public void TestGetDependents_SingleDependency()
        {
            graph.AddDependency("A", "B");
            var dependents = graph.GetDependents("A");
            CollectionAssert.AreEquivalent(new List<string> { "B" }, new List<string>(dependents));
        }

        [TestMethod]
        public void TestGetDependents_MultipleDependencies()
        {
            graph.AddDependency("A", "B");
            graph.AddDependency("A", "C");
            var dependents = graph.GetDependents("A");
            CollectionAssert.AreEquivalent(new List<string> { "B", "C" }, new List<string>(dependents));
        }

        [TestMethod]
        public void TestGetDependents_NoDependents()
        {
            var dependents = graph.GetDependents("A");
            CollectionAssert.AreEquivalent(new List<string>(), new List<string>(dependents));
        }

        [TestMethod]
        public void TestGetDependents_EmptyGraph()
        {
            var dependents = graph.GetDependents("NonExistentNode");
            CollectionAssert.AreEquivalent(new List<string>(), new List<string>(dependents));
        }

        [TestMethod]
        public void TestGetDependents_AfterRemovingDependency()
        {
            graph.AddDependency("A", "B");
            graph.RemoveDependency("A", "B");
            var dependents = graph.GetDependents("A");
            CollectionAssert.AreEquivalent(new List<string>(), new List<string>(dependents));
        }

        // Tests for GetDependees Method
        [TestMethod]
        public void TestGetDependees_SingleDependee()
        {
            graph.AddDependency("A", "B");
            var dependees = graph.GetDependees("B");
            CollectionAssert.AreEquivalent(new List<string> { "A" }, new List<string>(dependees));
        }

        [TestMethod]
        public void TestGetDependees_MultipleDependees()
        {
            graph.AddDependency("A", "B");
            graph.AddDependency("C", "B");
            var dependees = graph.GetDependees("B");
            CollectionAssert.AreEquivalent(new List<string> { "A", "C" }, new List<string>(dependees));
        }

        [TestMethod]
        public void TestGetDependees_NoDependees()
        {
            var dependees = graph.GetDependees("A");
            CollectionAssert.AreEquivalent(new List<string>(), new List<string>(dependees));
        }

        [TestMethod]
        public void TestGetDependees_EmptyGraph()
        {
            var dependees = graph.GetDependees("NonExistentNode");
            CollectionAssert.AreEquivalent(new List<string>(), new List<string>(dependees));
        }

        [TestMethod]
        public void TestGetDependees_AfterRemovingDependency()
        {
            graph.AddDependency("A", "B");
            graph.RemoveDependency("A", "B");
            var dependees = graph.GetDependees("B");
            CollectionAssert.AreEquivalent(new List<string>(), new List<string>(dependees));
        }

        // Tests for ReplaceDependents method
        [TestMethod]
        public void TestReplaceDependents_ExistingDependents()
        {
            graph.AddDependency("A", "B");
            graph.AddDependency("A", "C");

            graph.ReplaceDependents("A", new List<string> { "D", "E" });

            var dependents = graph.GetDependents("A");
            CollectionAssert.AreEquivalent(new List<string> { "D", "E" }, new List<string>(dependents));
        }

        [TestMethod]
        public void TestReplaceDependents_NoPriorDependents()
        {
            graph.ReplaceDependents("A", new List<string> { "B", "C" });

            var dependents = graph.GetDependents("A");
            CollectionAssert.AreEquivalent(new List<string> { "B", "C" }, new List<string>(dependents));
        }

        [TestMethod]
        public void TestReplaceDependents_ClearDependents()
        {
            graph.AddDependency("A", "B");
            graph.AddDependency("A", "C");

            graph.ReplaceDependents("A", new List<string>());

            var dependents = graph.GetDependents("A");
            CollectionAssert.AreEquivalent(new List<string>(), new List<string>(dependents));
        }

        [TestMethod]
        public void TestReplaceDependents_NonExistentNode()
        {
            graph.ReplaceDependents("X", new List<string> { "A", "B" });

            var dependents = graph.GetDependents("X");
            CollectionAssert.AreEquivalent(new List<string> { "A", "B" }, new List<string>(dependents));
        }

        [TestMethod]
        public void TestReplaceDependents_SelfDependency()
        {
            graph.ReplaceDependents("A", new List<string> { "A" });

            var dependents = graph.GetDependents("A");
            CollectionAssert.AreEquivalent(new List<string> { "A" }, new List<string>(dependents));

            var dependees = graph.GetDependees("A");
            CollectionAssert.AreEquivalent(new List<string> { "A" }, new List<string>(dependees));
        }

        // Tests for ReplaceDependees method
        [TestMethod]
        public void TestReplaceDependees_ExistingDependees()
        {
            graph.AddDependency("B", "A");
            graph.AddDependency("C", "A");

            graph.ReplaceDependees("A", new List<string> { "D", "E" });

            var dependees = graph.GetDependees("A");
            CollectionAssert.AreEquivalent(new List<string> { "D", "E" }, new List<string>(dependees));
        }

        [TestMethod]
        public void TestReplaceDependees_NoPriorDependees()
        {
            graph.ReplaceDependees("A", new List<string> { "B", "C" });

            var dependees = graph.GetDependees("A");
            CollectionAssert.AreEquivalent(new List<string> { "B", "C" }, new List<string>(dependees));
        }

        [TestMethod]
        public void TestReplaceDependees_ClearDependees()
        {
            graph.AddDependency("B", "A");
            graph.AddDependency("C", "A");

            graph.ReplaceDependees("A", new List<string>());

            var dependees = graph.GetDependees("A");
            CollectionAssert.AreEquivalent(new List<string>(), new List<string>(dependees));
        }

        [TestMethod]
        public void TestReplaceDependees_NonExistentNode()
        {
            graph.ReplaceDependees("X", new List<string> { "A", "B" });

            var dependees = graph.GetDependees("X");
            CollectionAssert.AreEquivalent(new List<string> { "A", "B" }, new List<string>(dependees));
        }

        [TestMethod]
        public void TestReplaceDependees_SelfDependency()
        {
            graph.ReplaceDependees("A", new List<string> { "A" });

            var dependees = graph.GetDependees("A");
            CollectionAssert.AreEquivalent(new List<string> { "A" }, new List<string>(dependees));

            var dependents = graph.GetDependents("A");
            CollectionAssert.AreEquivalent(new List<string> { "A" }, new List<string>(dependents));
        }
    }
}
