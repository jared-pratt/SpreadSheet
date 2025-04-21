///
///<authors> Jared Pratt </authors>
///<version> October 19, 2024 </version>
///

using CS3500.Formula;
using CS3500.Spreadsheet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        #region GetNamesOfAllNonemptyCells Tests

        [TestMethod]
        public void GetNamesOfAllNonemptyCells_EmptySpreadsheet_ReturnsEmptySet()
        {
            Spreadsheet ss = new Spreadsheet();
            ISet<string> nonEmptyCells = ss.GetNamesOfAllNonemptyCells();
            Assert.AreEqual(0, nonEmptyCells.Count);
        }

        [TestMethod]
        public void GetNamesOfAllNonemptyCells_OneNonEmptyCell_ReturnsSetWithOneCell()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "hello");
            ISet<string> nonEmptyCells = ss.GetNamesOfAllNonemptyCells();
            Assert.AreEqual(1, nonEmptyCells.Count);
            Assert.IsTrue(nonEmptyCells.Contains("A1"));
        }

        [TestMethod]
        public void GetNamesOfAllNonemptyCells_MultipleNonEmptyCells_ReturnsAllNonEmptyCells()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "hello");
            ss.SetContentsOfCell("B1", "5.0");
            ss.SetContentsOfCell("C1", "=A1+B1");
            ISet<string> nonEmptyCells = ss.GetNamesOfAllNonemptyCells();
            Assert.AreEqual(3, nonEmptyCells.Count);
            Assert.IsTrue(nonEmptyCells.Contains("A1"));
            Assert.IsTrue(nonEmptyCells.Contains("B1"));
            Assert.IsTrue(nonEmptyCells.Contains("C1"));
        }

        [TestMethod]
        public void GetNamesOfAllNonemptyCells_SetCellToEmpty_RemovesCellFromNonEmptyCells()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "hello");
            ss.SetContentsOfCell("B1", "5.0");
            ss.SetContentsOfCell("A1", "");
            ISet<string> nonEmptyCells = ss.GetNamesOfAllNonemptyCells();
            Assert.AreEqual(1, nonEmptyCells.Count);
            Assert.IsFalse(nonEmptyCells.Contains("A1"));
            Assert.IsTrue(nonEmptyCells.Contains("B1"));
        }

        [TestMethod]
        public void GetNamesOfAllNonemptyCells_CaseInsensitiveCellNames()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "hello");
            ss.SetContentsOfCell("A1", "world");
            ISet<string> nonEmptyCells = ss.GetNamesOfAllNonemptyCells();
            Assert.AreEqual(1, nonEmptyCells.Count);
            Assert.IsTrue(nonEmptyCells.Contains("A1"));
            Assert.IsFalse(nonEmptyCells.Contains("a1"));
        }

        #endregion

        #region GetCellContents Tests

        [TestMethod]
        public void GetCellContents_EmptyCell_ReturnsEmptyString()
        {
            Spreadsheet ss = new Spreadsheet();
            object contents = ss.GetCellContents("A1");
            Assert.AreEqual("", contents);
        }

        [TestMethod]
        public void GetCellContents_CellWithString_ReturnsString()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "hello");
            object contents = ss.GetCellContents("A1");
            Assert.AreEqual("hello", contents);
        }

        [TestMethod]
        public void GetCellContents_CellWithDouble_ReturnsDouble()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "5.0");
            object contents = ss.GetCellContents("A1");
            Assert.AreEqual(5.0, contents);
        }

        [TestMethod]
        public void GetCellContents_CellWithFormula_ReturnsFormula()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=B1 + C1");
            object contents = ss.GetCellContents("A1");
            Assert.IsInstanceOfType(contents, typeof(Formula));
            Assert.AreEqual(new Formula("B1 + C1"), contents);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContents_InvalidCellName_ThrowsInvalidNameException()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents("1A");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContents_EmptyCellName_ThrowsInvalidNameException()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents("");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContents_InvalidCharactersInName_ThrowsInvalidNameException()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents("A1$");
        }

        [TestMethod]
        public void GetCellContents_CaseInsensitiveCellNames()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "hello");
            object contents = ss.GetCellContents("A1");
            Assert.AreEqual("hello", contents);
        }

        #endregion

        #region SetContentsOfCell Tests

        [TestMethod]
        public void SetContentsOfCell_Number_CellContentsAreSet()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "5.0");
            object contents = ss.GetCellContents("A1");
            Assert.AreEqual(5.0, contents);
        }

        [TestMethod]
        public void SetContentsOfCell_Text_CellContentsAreSet()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "hello");
            object contents = ss.GetCellContents("A1");
            Assert.AreEqual("hello", contents);
        }

        [TestMethod]
        public void SetContentsOfCell_Formula_CellContentsAreSet()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=B1 + C1");
            object contents = ss.GetCellContents("A1");
            Assert.IsInstanceOfType(contents, typeof(Formula));
            Assert.AreEqual(new Formula("B1 + C1"), contents);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SetContentsOfCell_Formula_CircularDependency_ThrowsCircularException()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=B1");
            ss.SetContentsOfCell("B1", "=C1");
            ss.SetContentsOfCell("C1", "=A1"); // Circular dependency here
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SetContentsOfCell_InvalidFormula_ThrowsFormulaFormatException()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=1++2"); // Invalid formula
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCell_InvalidCellName_ThrowsInvalidNameException()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("1A", "5.0");
        }

        [TestMethod]
        public void SetContentsOfCell_CaseInsensitiveCellName()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "5.0");
            object contents = ss.GetCellContents("A1");
            Assert.AreEqual(5.0, contents);
        }

        [TestMethod]
        public void SetContentsOfCell_ReplacesExistingDependencies()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=B1 + C1");
            ss.SetContentsOfCell("A1", "=D1 + E1");

            ss.SetContentsOfCell("D1", "2.0");
            ss.SetContentsOfCell("E1", "3.0");
            IList<string> cellsToRecalculate = ss.SetContentsOfCell("D1", "5.0");
            Assert.IsTrue(cellsToRecalculate.Contains("D1"));
            Assert.IsTrue(cellsToRecalculate.Contains("A1"));
        }

        #endregion

        #region Save and Load Tests

        [TestMethod]
        public void SaveSpreadsheet_SavesCorrectly()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "5.0");
            ss.SetContentsOfCell("B1", "=A1+2");
            ss.SetContentsOfCell("C1", "hello");

            string filename = "testSave.txt";
            ss.Save(filename);

            Assert.IsFalse(ss.Changed);
            Assert.IsTrue(File.Exists(filename));

            // Clean up
            File.Delete(filename);
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveSpreadsheet_InvalidPath_ThrowsSpreadsheetReadWriteException()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "5.0");
            ss.Save("/some/nonsense/path.txt"); // Invalid path on both Windows and Linux
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void LoadSpreadsheet_InvalidFile_ThrowsSpreadsheetReadWriteException()
        {
            string filename = "nonExistentFile.txt";
            Spreadsheet ss = new Spreadsheet(filename);
        }

        #endregion

        #region Stress Tests

        [TestMethod]
        public void StressTest_LargeNumberOfCells()
        {
            Spreadsheet ss = new Spreadsheet();

            // Set up a chain of dependencies
            int size = 1000;
            ss.SetContentsOfCell("A1", "1");
            for (int i = 2; i <= size; i++)
            {
                ss.SetContentsOfCell($"A{i}", $"=A{i - 1}+1");
            }

            // Change the value of A1 and ensure all cells are recalculated
            ss.SetContentsOfCell("A1", "2");

            Assert.AreEqual(2.0, ss.GetCellValue("A1"));
            for (int i = 2; i <= size; i++)
            {
                Assert.AreEqual((double)(i + 1), ss.GetCellValue($"A{i}"));
            }
        }

        [TestMethod]
        public void StressTest_WideDependencyGraph()
        {
            Spreadsheet ss = new Spreadsheet();

            // A1 depends on B1, C1, ..., Z1
            for (char c = 'B'; c <= 'Z'; c++)
            {
                ss.SetContentsOfCell($"{c}1", "1");
            }
            ss.SetContentsOfCell("A1", "=B1+C1+D1+E1+F1+G1+H1+I1+J1+K1+L1+M1+N1+O1+P1+Q1+R1+S1+T1+U1+V1+W1+X1+Y1+Z1");

            // Change one of the dependencies
            ss.SetContentsOfCell("B1", "2");

            Assert.AreEqual(26.0, ss.GetCellValue("A1"));
        }

        #endregion

        #region Additional Tests for Code Coverage

        [TestMethod]
        public void GetCellValue_UndefinedVariableInFormula_ReturnsFormulaError()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=B1+1");
            object value = ss.GetCellValue("A1");
            Assert.IsInstanceOfType(value, typeof(FormulaError));
        }

        [TestMethod]
        public void GetCellValue_DivideByZeroInFormula_ReturnsFormulaError()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "0");
            ss.SetContentsOfCell("B1", "=1/A1");
            object value = ss.GetCellValue("B1");
            Assert.IsInstanceOfType(value, typeof(FormulaError));
        }

        [TestMethod]
        public void ChangedProperty_IsSetCorrectly()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);
            ss.SetContentsOfCell("A1", "5");
            Assert.IsTrue(ss.Changed);
            ss.Save("testChanged.txt");
            Assert.IsFalse(ss.Changed);

            // Clean up
            File.Delete("testChanged.txt");
        }

        #endregion

        #region Indexer Tests

        [TestMethod]
        public void Indexer_ValidCellName_ReturnsCellValue()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "5");
            object value = ss["A1"];
            Assert.AreEqual(5.0, value);
        }

        [TestMethod]
        public void Indexer_CellWithFormula_ReturnsEvaluatedValue()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "5");
            ss.SetContentsOfCell("B1", "=A1+2");
            object value = ss["B1"];
            Assert.AreEqual(7.0, value);
        }

        [TestMethod]
        public void Indexer_CellWithString_ReturnsStringValue()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "hello");
            object value = ss["A1"];
            Assert.AreEqual("hello", value);
        }

        [TestMethod]
        public void Indexer_EmptyCell_ReturnsEmptyString()
        {
            Spreadsheet ss = new Spreadsheet();
            object value = ss["A1"];
            Assert.AreEqual("", value);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Indexer_InvalidCellName_ThrowsInvalidNameException()
        {
            Spreadsheet ss = new Spreadsheet();
            var value = ss["1A"]; // Invalid cell name
        }

        [TestMethod]
        public void Indexer_UndefinedVariableInFormula_ReturnsFormulaError()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("B1", "=C1+2"); // C1 is undefined
            object value = ss["B1"];
            Assert.IsInstanceOfType(value, typeof(FormulaError));
        }

        [TestMethod]
        public void Indexer_DivideByZeroInFormula_ReturnsFormulaError()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "0");
            ss.SetContentsOfCell("B1", "=1/A1");
            object value = ss["B1"];
            Assert.IsInstanceOfType(value, typeof(FormulaError));
        }

        [TestMethod]
        public void Indexer_CircularDependency_ExceptionLeavesSpreadsheetUnchanged()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=B1");

            try
            {
                ss.SetContentsOfCell("B1", "=A1"); // This line will throw CircularException
                Assert.Fail("Expected CircularException was not thrown.");
            }
            catch (CircularException)
            {
                // Exception was thrown as expected
            }

            // Verify that B1's contents were not changed
            Assert.AreEqual("", ss.GetCellContents("B1"));
            // Verify that A1's value is a FormulaError because B1 is empty
            object value = ss["A1"];
            Assert.IsInstanceOfType(value, typeof(FormulaError));
        }

        #endregion

        #region Load Spreadsheet Tests

        [TestMethod]
        public void LoadSpreadsheet_LoadedSpreadsheetIsNull_DoesNotThrow()
        {
            // Arrange
            string filename = "nullSpreadsheet.txt";
            string json = "null"; // JSON deserialization of 'null' results in null object

            File.WriteAllText(filename, json);

            // Act
            Spreadsheet ss = new Spreadsheet(filename);

            // Assert
            // No exception should be thrown
            Assert.IsFalse(ss.Changed);
            Assert.AreEqual(0, ss.GetNamesOfAllNonemptyCells().Count);

            // Clean up
            File.Delete(filename);
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void LoadSpreadsheet_MalformedJson_ThrowsSpreadsheetReadWriteException()
        {
            // Arrange
            string filename = "malformedJson.txt";
            string json = "{ invalid json }"; // Malformed JSON

            File.WriteAllText(filename, json);

            // Act
            Spreadsheet ss = new Spreadsheet(filename);

            // Clean up
            File.Delete(filename);
        }

        #endregion

        [TestMethod]
        public void GetCellValue_FormulaReferencesStringCell_ReturnsFormulaError()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "hello");
            ss.SetContentsOfCell("B1", "=A1 + 2");
            object value = ss.GetCellValue("B1");
            Assert.IsInstanceOfType(value, typeof(FormulaError));
        }

        [TestMethod]
        public void GetCellValue_FormulaReferencesErrorCell_ReturnsFormulaError()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=1/0"); // Division by zero
            ss.SetContentsOfCell("B1", "=A1 + 2");
            object value = ss.GetCellValue("B1");
            Assert.IsInstanceOfType(value, typeof(FormulaError));
        }

        /// <summary>
        /// Helper method to verify an arbitrary spreadsheet's values
        /// Cell names and eexpeced values are given in an array in alternating pairs
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="constraints"></param>
        public void VerifyValues(Spreadsheet sheet, params object[] constraints)
        {
            for (int i = 0; i < constraints.Length; i += 2)
            {
                if (constraints[i + 1] is double)
                {
                    Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
                }
                else
                {
                    Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
                }
            }
        }


        /// <summary>
        /// Helper method to set the contents of a given cell for a given spreadsheet
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="name"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        public IEnumerable<string> Set(Spreadsheet sheet, string name, string contents)
        {
            List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
            return result;
        }

        // Tests IsValid
        [TestMethod, Timeout(2000)]
        [TestCategory("1")]
        public void SetContentsOfCell_SetString_IsValid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "x");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("2")]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCell_InvalidName_Throws()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("1a", "x");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("3")]
        public void SetContentsOfCell_SetFormula_IsValid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "= A1 + C1");
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("4")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SetContentsOfCell_SetInvalidFormula_Throws() // try construct an invalid formula
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("B1", "= A1 + 1C");
        }

        // Tests Normalize
        [TestMethod, Timeout(2000)]
        [TestCategory("5")]
        public void GetCellContents_LowerCaseName_IsValid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("hello", s.GetCellContents("b1"));
        }

        /// <summary>
        /// Increase the weight by repeating the previous test
        /// </summary>
        [TestMethod, Timeout(2000)]
        [TestCategory("6")]
        public void GetCellContents_LowerCaseName_IsValid2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("hello", ss.GetCellContents("b1"));
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("7")]
        public void GetCellValue_CaseSensitivity_IsValid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            s.SetContentsOfCell("B1", "= A1");
            Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("8")]
        public void GetCellValue_CaseSensitivity_IsValid2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "5");
            ss.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(5.0, (double)ss.GetCellValue("B1"), 1e-9);
        }

        // Simple tests
        [TestMethod, Timeout(2000)]
        [TestCategory("9")]
        public void Constructor_Empty_CorrectValue()
        {
            Spreadsheet ss = new Spreadsheet();
            VerifyValues(ss, "A1", "");
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("10")]
        public void GetCellValue_GetString_IsValid()
        {
            Spreadsheet ss = new Spreadsheet();
            OneString(ss);
        }

        /// <summary>
        /// Helper method that sets one string in one cell and verifies the value
        /// </summary>
        /// <param name="ss"></param>
        public void OneString(Spreadsheet ss)
        {
            Set(ss, "B1", "hello");
            VerifyValues(ss, "B1", "hello");
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("11")]
        public void GetCellValue_GetNumber_IsValid()
        {
            Spreadsheet ss = new Spreadsheet();
            OneNumber(ss);
        }

        /// <summary>
        /// Helper method that sets one number in one cell and verifies the value
        /// </summary>
        /// <param name="ss"></param>
        public void OneNumber(Spreadsheet ss)
        {
            Set(ss, "C1", "17.5");
            VerifyValues(ss, "C1", 17.5);
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("12")]
        public void GetCellValue_GetFormula_IsValid()
        {
            Spreadsheet ss = new Spreadsheet();
            OneFormula(ss);
        }

        /// <summary>
        /// Helper method that sets one formula in one cell and verifies the value
        /// </summary>
        /// <param name="ss"></param>
        public void OneFormula(Spreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "5.2");
            Set(ss, "C1", "= A1+B1");
            VerifyValues(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("13")]
        public void Changed_AfterModify_IsTrue()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);
            Set(ss, "C1", "17.5");
            Assert.IsTrue(ss.Changed);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("13b")]
        public void Changed_AfterSave_IsFalse()
        {
            Spreadsheet ss = new Spreadsheet();
            Set(ss, "C1", "17.5");
            ss.Save("changed.txt");
            Assert.IsFalse(ss.Changed);
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("14")]
        public void GetCellValue_DivideByZero_ReturnsError()
        {
            Spreadsheet ss = new Spreadsheet();
            DivisionByZero1(ss);
        }

        /// <summary>
        /// Helper method to test a formula that indirectly divides by zero
        /// </summary>
        /// <param name="ss"></param>
        public void DivisionByZero1(Spreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "0.0");
            Set(ss, "C1", "= A1 / B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("15")]
        public void GetCellValue_DivideByZero_ReturnsError2()
        {
            Spreadsheet ss = new Spreadsheet();
            DivisionByZero2(ss);
        }

        /// <summary>
        /// Helper method that directly divides by zero
        /// </summary>
        /// <param name="ss"></param>
        public void DivisionByZero2(Spreadsheet ss)
        {
            Set(ss, "A1", "5.0");
            Set(ss, "A3", "= A1 / 0.0");
            Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
        }



        [TestMethod, Timeout(2000)]
        [TestCategory("16")]
        public void GetCellValue_FormulaBadVariable_ReturnsError()
        {
            Spreadsheet ss = new Spreadsheet();
            EmptyArgument(ss);
        }

        /// <summary>
        /// Helper method that tests a formula that references an empty cell
        /// </summary>
        /// <param name="ss"></param>
        public void EmptyArgument(Spreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("17")]
        public void GetCellValue_FormulaBadVariable_ReturnsError2()
        {
            Spreadsheet ss = new Spreadsheet();
            StringArgument(ss);
        }

        /// <summary>
        /// Helper method that tests a formula that references a non-empty string cell
        /// </summary>
        /// <param name="ss"></param>
        public void StringArgument(Spreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "hello");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("18")]
        public void GetCellValue_FormulaIndirectBadVariable_ReturnsError()
        {
            Spreadsheet ss = new Spreadsheet();
            ErrorArgument(ss);
        }

        /// <summary>
        /// Helper method that creates a formula that indirectly references an empty cell
        /// </summary>
        /// <param name="ss"></param>
        public void ErrorArgument(Spreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= C1");
            Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("19")]
        public void GetCellValue_FormulaWithVariable_IsValid()
        {
            Spreadsheet ss = new Spreadsheet();
            NumberFormula1(ss);
        }

        /// <summary>
        /// Helper method that creates a simple formula with a variable reference
        /// </summary>
        /// <param name="ss"></param>
        public void NumberFormula1(Spreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + 4.2");
            VerifyValues(ss, "C1", 8.3);
        }


        [TestMethod, Timeout(2000)]
        [TestCategory("20")]
        public void GetCellValue_FormulaWithNumber_IsValid()
        {
            Spreadsheet ss = new Spreadsheet();
            NumberFormula2(ss);
        }

        /// <summary>
        /// Helper method that creates a simple formula that's just a number
        /// </summary>
        /// <param name="ss"></param>
        public void NumberFormula2(Spreadsheet ss)
        {
            Set(ss, "A1", "= 4.6");
            VerifyValues(ss, "A1", 4.6);
        }


        // Repeats the simple tests all together
        [TestMethod, Timeout(2000)]
        [TestCategory("21")]
        public void StressTestVariety()
        {
            Spreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "17.32");
            Set(ss, "B1", "This is a test");
            Set(ss, "C1", "= A1+B1");
            OneString(ss);
            OneNumber(ss);
            OneFormula(ss);
            DivisionByZero1(ss);
            DivisionByZero2(ss);
            StringArgument(ss);
            ErrorArgument(ss);
            NumberFormula1(ss);
            NumberFormula2(ss);
        }

        // Four kinds of formulas
        [TestMethod, Timeout(2000)]
        [TestCategory("22")]
        public void StressTestFormulas()
        {
            Spreadsheet ss = new Spreadsheet();
            Formulas(ss);
        }

        public void Formulas(Spreadsheet ss)
        {
            Set(ss, "A1", "4.4");
            Set(ss, "B1", "2.2");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= A1 - B1");
            Set(ss, "E1", "= A1 * B1");
            Set(ss, "F1", "= A1 / B1");
            VerifyValues(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
        }
    }
}
