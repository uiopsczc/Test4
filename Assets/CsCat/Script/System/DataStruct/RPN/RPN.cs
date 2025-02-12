using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace CsCat
{
	/// <summary>
	/// <summary>
	/// 逆波兰表达式，牛x得很,只能用于计算数学运算，计算数值
	/// Summary description for RPNParser.
	/// http://www.codeproject.com/Articles/5875/C-Expression-Parser-using-RPN
	/// </summary>
	public class RPN
	{
		#region ctor

		public RPN()
		{
		}

		#endregion

		public object Evaluate(string exp)
		{
			exp = exp.ToLower();
			exp = exp.Replace("false", "0");
			exp = exp.Replace("true", "1");
			return EvaluateExpression(exp, Type.GetType("System.Int64"), false, null);
		}

		public object EvaluateExpression(string szExpr, Type varType, bool bFormula, Hashtable htValues)
		{
			ArrayList arrExpr = GetPostFixNotation(szExpr, varType, bFormula);
			return EvaluateRPN(arrExpr, varType, htValues);
		}

		public object EvaluateExpression(string szExpr)
		{
			long myType = 0;
			ArrayList arrExpr = GetPostFixNotation(szExpr, myType.GetType(), false);
			return EvaluateRPN(arrExpr, myType.GetType(), null);
		}

		#region replacement for string

		public string replacementToRPN(string expression)
		{
			while (Regex.IsMatch(expression, "\"[^\"]*\""))
			{
				string myString = Regex.Match(expression, "\"[^\"]*\"").Value.ToString();
				string stringAdapted = myString.Replace("\"", "@@@");
				stringAdapted = stringAdapted.Replace("+", "µ#1");
				stringAdapted = stringAdapted.Replace("-", "µ#2");
				stringAdapted = stringAdapted.Replace("/", "µ#3");
				stringAdapted = stringAdapted.Replace("*", "µ#4");
				stringAdapted = stringAdapted.Replace(")", "µ#5");
				stringAdapted = stringAdapted.Replace("(", "µ#6");
				stringAdapted = stringAdapted.Replace("%", "µ#7");
				stringAdapted = stringAdapted.Replace("~", "µ#8");
				stringAdapted = stringAdapted.Replace("=", "µ#9");
				stringAdapted = stringAdapted.Replace("<", "µ#10");
				stringAdapted = stringAdapted.Replace(">", "µ#11");
				stringAdapted = stringAdapted.Replace("|", "µ#12");
				stringAdapted = stringAdapted.Replace("&", "µ#13");
				expression = expression.Replace(myString, stringAdapted);
			}

			expression = expression.Replace("@@@", "\"");
			return expression;
		}

		#endregion

		#region RPN_Parser

		///
		/// Algo of GetPostFixNotation (source : Expression Evaluator : using RPN by lallous
		/// in the C++/MFC section of www.CodeProject.com.
		/// 1. Initialize an empty stack (string stack), prepare input infix expression and clear RPN string
		/// 2. Repeat until we reach end of infix expression
		/// I. Get token (operand or operator); skip white spaces
		/// II. If token is:
		/// a. Left parenthesis: Push it into stack
		/// b. Right parenthesis: Keep popping from the stack and appending to
		/// RPN string until we reach the left parenthesis.
		/// If stack becomes empty and we didn't reach the left parenthesis
		/// then break out with error "Unbalanced parenthesis"
		/// c. Operator: If stack is empty or operator has a higher precedence than
		/// the top of the stack then push operator into stack.
		/// Else if operator has lower precedence then we keep popping and
		/// appending to RPN string, this is repeated until operator in stack
		/// has lower precedence than the current operator.
		/// d. An operand: we simply append it to RPN string.
		/// III. When the infix expression is finished, we start popping off the stack and
		/// appending to RPN string till stack becomes empty.
		///
		///
		///
		///
		public ArrayList GetPostFixNotation(string szExpr, Type varType, bool bFormula)
		{
			szExpr = this.replacementToRPN(szExpr);
			//Replace the +,/,-,*,% in a string
			Stack stkOp = new Stack();
			ArrayList arrFinalExpr = new ArrayList();
			string szResult = "";
			string szLastItem = "";
			//bool bApplyNegative = false;

			Tokenizer tknzr = null;
			switch (varType.ToString())
			{
				case "System.Int32":
				case "System.Int64":
					tknzr = new Tokenizer(szExpr, ExpressionType.ET_ARITHMETIC |
												  ExpressionType.ET_COMPARISON |
												  ExpressionType.ET_LOGICAL);
					break;
				case "System.String":
					tknzr = new Tokenizer(szExpr, ExpressionType.ET_COMPARISON);
					break;
			}


			foreach (Token token in tknzr)
			{
				string szToken = token.Value.Trim();
				if (szToken.Length == 0)
					continue;
				switch (varType.ToString())
				{
					case "System.Int32":
					case "System.Int64":
						if (!OperatorHelper.IsOperator(szToken))
						{
							Operand oprnd;
							//if contains " then it's a string type
							if (szToken.IndexOf("\"") != -1)
							{
								string myStringType = "";
								oprnd = OperandHelper.CreateOperand(szToken, myStringType.GetType());
							}
							else oprnd = OperandHelper.CreateOperand(szToken, varType);

							oprnd.ExtractAndSetValue(szToken, bFormula);
							arrFinalExpr.Add(oprnd);

							szResult += szToken;
							szLastItem = szToken;
							continue;
						}

						break;
					case "System.String":
						if (!OperatorHelper.IsComparisonOperator(szToken))
						{
							Operand oprnd = OperandHelper.CreateOperand(szToken, varType);
							oprnd.ExtractAndSetValue(szToken, bFormula);
							arrFinalExpr.Add(oprnd);

							szResult += szToken;
							szLastItem = szToken;
							continue;
						}

						break;
				}

				string szOp = szToken;
				if (szOp == "(")
				{
					stkOp.Push(szOp);
				}
				else if (szOp == "!")
				{
					stkOp.Push(szOp);
				}
				else if (szOp == ")")
				{
					string szTop;
					while ((szTop = (string)stkOp.Pop()) != "(")
					{
						IOperator oprtr = OperatorHelper.CreateOperator(szTop);
						arrFinalExpr.Add(oprtr);

						szResult += szTop;

						if (stkOp.Count == 0)
							throw new RPN_Exception("Unmatched braces!");
					}
				}
				else
				{
					if (szOp == "-" && (!StringContentCheck.IsAlphaNumeric(szLastItem) || szLastItem == ""))
					{
						//bApplyNegative = true;
						//Operand oprnd = OperandHelper.CreateOperand("0", varType);
						//oprnd.ExtractAndSetValue("0", bFormula);
						//arrFinalExpr.Add(oprnd);

						//szResult += "0";
						//stkOp.Push("(");
						stkOp.Push("~");
						//string szTop;
						//while ((szTop = (string)stkOp.Pop()) != "(")
						//{
						// IOperator oprtr = OperatorHelper.CreateOperator(szTop);
						// arrFinalExpr.Add(oprtr);

						// szResult += szTop;

						// if (stkOp.Count == 0)
						// throw new RPN_Exception("Unmatched braces!");
						//}
					}
					else
					{
						if (stkOp.Count == 0 || (string)stkOp.Peek() == "("
											 || OperatorHelper.IsHigherPrecOperator(szOp, (string)stkOp.Peek()))
						{
							stkOp.Push(szOp);
						}
						else
						{
							while (stkOp.Count != 0)
							{
								if (OperatorHelper.IsLowerPrecOperator(szOp, (string)stkOp.Peek())
									|| OperatorHelper.IsEqualPrecOperator(szOp, (string)stkOp.Peek()))
								{
									string szTop = (string)stkOp.Peek();
									if (szTop == "(")
										break;
									szTop = (string)stkOp.Pop();

									IOperator oprtr = OperatorHelper.CreateOperator(szTop);
									arrFinalExpr.Add(oprtr);

									szResult += szTop;
								}
								else
									break;
							}

							stkOp.Push(szOp);
						}
					}
				}

				szLastItem = szToken;
			}

			while (stkOp.Count != 0)
			{
				string szTop = (string)stkOp.Pop();
				if (szTop == "(")
					throw new RPN_Exception("Unmatched braces");

				IOperator oprtr = OperatorHelper.CreateOperator(szTop);
				arrFinalExpr.Add(oprtr);

				szResult += szTop;
			}

			return arrFinalExpr;
		}

		#endregion

		public string Convert2String(ArrayList arrExpr)
		{
			string szResult = "";
			foreach (object obj in arrExpr)
			{
				szResult += obj.ToString();
			}

			return szResult;
		}


		#region RPN_Evaluator

		///
		/// Algo of EvaluateRPN (source : Expression Evaluator : using RPN by lallous
		/// in the C++/MFC section of www.CodeProject.com.
		/// 1. Initialize stack for storing results, prepare input postfix (or RPN) expression.
		/// 2. Start scanning from left to right till we reach end of RPN expression
		/// 3. Get token, if token is:
		/// I. An operator:
		/// a. Get top of stack and store into variable op2; Pop the stack
		/// b. Get top of stack and store into variable op1; Pop the stack
		/// c. Do the operation expression in operator on both op1 and op2
		/// d. Push the result into the stack
		/// II. An operand: stack its numerical representation into our numerical stack.
		/// 4. At the end of the RPN expression, the stack should only have one value and
		/// that should be the result and can be retrieved from the top of the stack.
		///
		/// Expression to be evaluated in RPNotation with
		/// single character variables
		/// Values for each of the variables in the expression
		///
		public object EvaluateRPN(ArrayList arrExpr, Type varType, Hashtable htValues)
		{
			// initialize stack (integer stack) for results
			Stack stPad = new Stack();
			// begin loop : scan from left to right till end of RPN expression
			foreach (object var in arrExpr)
			{
				Operand op1 = null;
				Operand op2 = null;
				IOperator oprtr = null;
				// Get token
				// if token is
				if (var is IOperand)
				{
					// Operand : push onto top of numerical stack
					stPad.Push(var);
				}
				else if (var is IOperator)
				{
					if (OperatorHelper.IsUnaryOperator(((Operator)var).ToString()))
					{
						// Operator :
						// Pop top of stack into var 1
						op1 = (Operand)stPad.Pop();
						if (htValues != null)
						{
							op1.Value = htValues[op1.Name];
						}

						// Do operation exp for 'this' operator on var1 and var2
						oprtr = (IOperator)var;
						IOperand opRes = oprtr.Eval(op1);
						// Push results onto stack
						stPad.Push(opRes);
					}
					else
					{
						// Operator :
						// Pop top of stack into var 1 - op2 first as top of stack is rhs
						op2 = (Operand)stPad.Pop();
						if (htValues != null)
						{
							op2.Value = htValues[op2.Name];
						}

						// Pop top of stack into var 2
						op1 = (Operand)stPad.Pop();
						if (htValues != null)
						{
							op1.Value = htValues[op1.Name];
						}

						// Do operation exp for 'this' operator on var1 and var2
						oprtr = (IOperator)var;
						IOperand opRes = oprtr.Eval(op1, op2);
						// Push results onto stack
						stPad.Push(opRes);
					}
				}
			}

			// end loop
			// stack ends up with single value with final result
			return ((Operand)stPad.Pop()).Value;
		}

		#endregion
	}

	#region UtilClasses

	///
	/// The given expression can be parsed as either Arithmetic or Logical or
	/// Comparison ExpressionTypes. This is controlled by the enums
	/// ExpressionType::ET_ARITHMETIC, ExpressionType::ET_COMPARISON and
	/// ExpressionType::ET_LOGICAL. A combination of these enum types can also be given.
	/// E.g. To parse the expression as all of these, pass
	/// ExpressionType.ET_ARITHMETIC|ExpressionType.ET_COMPARISON|ExpressionType.ET_LOGICAL
	/// to the Tokenizer c'tor.
	///
	[Flags]
	public enum ExpressionType
	{
		ET_ARITHMETIC = 0x0001,
		ET_COMPARISON = 0x0002,
		ET_LOGICAL = 0x0004
	}

	///
	/// Currently not used.
	///
	public enum TokenType
	{
		TT_OPERATOR,
		TT_OPERAND
	}

	///
	/// Represents each token in the expression
	///
	public class Token
	{
		public Token(string szValue)
		{
			m_szValue = szValue;
		}

		public string Value
		{
			get { return m_szValue; }
		}

		string m_szValue;
	}

	///
	/// Is the tokenizer which does the actual parsing of the expression.
	///
	public class Tokenizer : IEnumerable
	{
		public Tokenizer(string szExpression)
			: this(szExpression, ExpressionType.ET_ARITHMETIC |
								 ExpressionType.ET_COMPARISON |
								 ExpressionType.ET_LOGICAL)
		{
		}

		public Tokenizer(string szExpression, ExpressionType exType)
		{
			m_szExpression = szExpression;
			m_exType = exType;
			m_RegEx = new Regex(OperatorHelper.GetOperatorsRegEx(m_exType));
			m_strarrTokens = SplitExpression(szExpression);
		}

		public IEnumerator GetEnumerator()
		{
			return new TokenEnumerator(m_strarrTokens);
		}

		public string[] SplitExpression(string szExpression)
		{
			return m_RegEx.Split(szExpression);
		}

		ExpressionType m_exType;
		string m_szExpression;
		string[] m_strarrTokens;
		Regex m_RegEx;
	}

	///
	/// Enumerator to enumerate over the tokens.
	///
	public class TokenEnumerator : IEnumerator
	{
		Token m_Token;
		int m_nIdx;
		string[] m_strarrTokens;

		public TokenEnumerator(string[] strarrTokens)
		{
			m_strarrTokens = strarrTokens;
			Reset();
		}

		public object Current
		{
			get { return m_Token; }
		}

		public bool MoveNext()
		{
			if (m_nIdx >= m_strarrTokens.Length)
				return false;

			m_Token = new Token(m_strarrTokens[m_nIdx]);
			m_nIdx++;
			return true;
		}

		public void Reset()
		{
			m_nIdx = 0;
		}
	}

	#region Exceptions

	///
	/// For the exceptions thrown by this module.
	///
	public class RPN_Exception : ApplicationException
	{
		public RPN_Exception()
		{
		}

		public RPN_Exception(string szMessage)
			: base(szMessage)
		{
		}

		public RPN_Exception(string szMessage, Exception innerException)
			: base(szMessage, innerException)
		{
		}
	}

	#endregion

	#endregion

	#region Interfaces

	public interface IOperand
	{
	}

	public interface IOperator
	{
		IOperand Eval(IOperand lhs, IOperand rhs);
		IOperand Eval(IOperand lhs);
	}

	public interface IArithmeticOperations
	{
		// to support {"+", "-", "~", "*", "/", "%"} operators
		IOperand Plus(IOperand rhs);
		IOperand Minus(IOperand rhs);
		IOperand Negate();
		IOperand Multiply(IOperand rhs);
		IOperand Divide(IOperand rhs);
		IOperand Modulo(IOperand rhs);

		IOperand Not();
	}

	public interface IComparisonOperations
	{
		// to support {"==", "!=","<", "<=", ">", ">="} operators
		IOperand EqualTo(IOperand rhs);
		IOperand NotEqualTo(IOperand rhs);
		IOperand LessThan(IOperand rhs);
		IOperand LessThanOrEqualTo(IOperand rhs);
		IOperand GreaterThan(IOperand rhs);
		IOperand GreaterThanOrEqualTo(IOperand rhs);
	}

	public interface ILogicalOperations
	{
		// to support {"||", "&&" } operators
		IOperand OR(IOperand rhs);
		IOperand AND(IOperand rhs);
	}

	#endregion

	#region Operands

	///
	/// Base class for all Operands. Provides datastorage
	///
	public abstract class Operand : IOperand
	{
		public Operand(string szVarName, object varValue)
		{
			m_szVarName = szVarName;
			m_VarValue = varValue;
		}

		public Operand(string szVarName)
		{
			m_szVarName = szVarName;
		}

		public override string ToString()
		{
			return m_szVarName;
		}

		public abstract void ExtractAndSetValue(string szValue, bool bFormula);

		public string Name
		{
			get { return m_szVarName; }
			set { m_szVarName = value; }
		}

		public object Value
		{
			get { return m_VarValue; }
			set { m_VarValue = value; }
		}

		protected string m_szVarName = "";
		protected object m_VarValue = null;
	}

	///
	/// Operand corresponding to the Long (Int32/Int64) datatypes.
	///
	public class LongOperand : Operand, IArithmeticOperations, IComparisonOperations
	{
		public LongOperand(string szVarName, object varValue)
			: base(szVarName, varValue)
		{
		}

		public LongOperand(string szVarName)
			: base(szVarName)
		{
		}

		public override string ToString()
		{
			return m_szVarName;
		}

		public override void ExtractAndSetValue(string szValue, bool bFormula)
		{
			m_VarValue = !bFormula ? Convert.ToInt64(szValue) : 0;
		}

		/// IArithmeticOperations methods. Return of these methods is again a LongOperand
		public IOperand Plus(IOperand rhs)
		{
			if (!(rhs is LongOperand))
				throw new RPN_Exception("Argument invalid in LongOperand.Plus : rhs");
			LongOperand oprResult = new LongOperand("Result", Type.GetType("System.Int64"));
			oprResult.Value = (long)this.Value + (long)((Operand)rhs).Value;
			return oprResult;
		}

		public IOperand Minus(IOperand rhs)
		{
			if (!(rhs is LongOperand))
				throw new RPN_Exception("Argument invalid in LongOperand.Minus : rhs");
			LongOperand oprResult = new LongOperand("Result", Type.GetType("System.Int64"));
			oprResult.Value = (long)this.Value - (long)((Operand)rhs).Value;
			return oprResult;
		}

		public IOperand Negate()
		{
			LongOperand oprResult = new LongOperand("Result", Type.GetType("System.Int64"));
			oprResult.Value = (long)this.Value * -1;
			return oprResult;
		}

		public IOperand Multiply(IOperand rhs)
		{
			if (!(rhs is LongOperand))
				throw new ArgumentException("Argument invalid in LongOperand.Multiply : rhs");
			LongOperand oprResult = new LongOperand("Result", Type.GetType("System.Int64"));
			oprResult.Value = (long)this.Value * (long)((Operand)rhs).Value;
			return oprResult;
		}

		public IOperand Divide(IOperand rhs)
		{
			if (!(rhs is LongOperand))
				throw new RPN_Exception("Argument invalid in LongOperand.Divide : rhs");
			LongOperand oprResult = new LongOperand("Result", Type.GetType("System.Int64"));
			oprResult.Value = (long)this.Value / (long)((Operand)rhs).Value;
			return oprResult;
		}

		public IOperand Modulo(IOperand rhs)
		{
			if (!(rhs is LongOperand))
				throw new RPN_Exception("Argument invalid in LongOperand.Modulo : rhs");
			LongOperand oprResult = new LongOperand("Result", Type.GetType("System.Int64"));
			oprResult.Value = (long)this.Value % (long)((Operand)rhs).Value;
			return oprResult;
		}

		public IOperand Not()
		{
			BoolOperand oprResult = new BoolOperand("Result", Type.GetType("System.Boolean"));

			switch (Convert.ToString(this.Value))
			{
				case "0":
					oprResult.Value = !(false);
					break;
				case "1":
					oprResult.Value = !(true);
					break;
				default:
					break;
			}

			return oprResult;
		}

		/// IComparisonOperators methods. Return values are always BooleanOperands type
		public IOperand EqualTo(IOperand rhs)
		{
			if (!(rhs is LongOperand))
				throw new RPN_Exception("Argument invalid in LongOperand.== : rhs");
			BoolOperand oprResult = new BoolOperand("Result");
			oprResult.Value = (long)this.Value == (long)((Operand)rhs).Value;
			return oprResult;
		}

		public IOperand NotEqualTo(IOperand rhs)
		{
			if (!(rhs is LongOperand))
				throw new RPN_Exception("Argument invalid in LongOperand.!= : rhs");
			BoolOperand oprResult = new BoolOperand("Result");
			oprResult.Value = ((long)this.Value != (long)((Operand)rhs).Value) ? true : false;
			return oprResult;
		}

		public IOperand LessThan(IOperand rhs)
		{
			if (!(rhs is LongOperand))
				throw new RPN_Exception("Argument invalid in LongOperand.< : rhs");
			BoolOperand oprResult = new BoolOperand("Result");
			oprResult.Value = ((long)this.Value < (long)((Operand)rhs).Value) ? true : false;
			return oprResult;
		}

		public IOperand LessThanOrEqualTo(IOperand rhs)
		{
			if (!(rhs is LongOperand))
				throw new RPN_Exception("Argument invalid in LongOperand.<= : rhs");
			BoolOperand oprResult = new BoolOperand("Result");
			oprResult.Value = ((long)this.Value <= (long)((Operand)rhs).Value) ? true : false;
			return oprResult;
		}

		public IOperand GreaterThan(IOperand rhs)
		{
			if (!(rhs is LongOperand))
				throw new RPN_Exception("Argument invalid in LongOperand.> : rhs");
			BoolOperand oprResult = new BoolOperand("Result");
			oprResult.Value = ((long)this.Value > (long)((Operand)rhs).Value) ? true : false;
			return oprResult;
		}

		public IOperand GreaterThanOrEqualTo(IOperand rhs)
		{
			if (!(rhs is LongOperand))
				throw new RPN_Exception("Argument invalid in LongOperand.>= : rhs");
			BoolOperand oprResult = new BoolOperand("Result");
			oprResult.Value = ((long)this.Value >= (long)((Operand)rhs).Value) ? true : false;
			return oprResult;
		}
	}

	public class DoubleOperand : Operand, IArithmeticOperations, IComparisonOperations
	{
		public DoubleOperand(string szVarName, object varValue)
			: base(szVarName, varValue)
		{
		}

		public DoubleOperand(string szVarName)
			: base(szVarName)
		{
		}

		public override string ToString()
		{
			return m_szVarName;
		}

		public override void ExtractAndSetValue(string szValue, bool bFormula)
		{
			m_VarValue = !bFormula ? Convert.ToDouble(szValue) : 0;
		}

		/// IArithmeticOperations methods. Return of these methods is again a LongOperand
		public IOperand Plus(IOperand rhs)
		{
			if (!(rhs is DoubleOperand))
				throw new RPN_Exception("Argument invalid in DoubleOperand.Plus : rhs");
			DoubleOperand oprResult = new DoubleOperand("Result", Type.GetType("System.Int64"));
			oprResult.Value = (double)this.Value + (double)((Operand)rhs).Value;
			return oprResult;
		}

		public IOperand Minus(IOperand rhs)
		{
			if (!(rhs is DoubleOperand))
				throw new RPN_Exception("Argument invalid in DoubleOperand.Minus : rhs");
			DoubleOperand oprResult = new DoubleOperand("Result", Type.GetType("System.Int64"));
			oprResult.Value = (double)this.Value - (double)((Operand)rhs).Value;
			return oprResult;
		}

		public IOperand Negate()
		{
			DoubleOperand oprResult = new DoubleOperand("Result", Type.GetType("System.Int64"));
			oprResult.Value = (double)this.Value * -1;
			return oprResult;
		}

		public IOperand Multiply(IOperand rhs)
		{
			if (!(rhs is DoubleOperand))
				throw new ArgumentException("Argument invalid in DoubleOperand.Multiply : rhs");
			DoubleOperand oprResult = new DoubleOperand("Result", Type.GetType("System.Int64"));
			oprResult.Value = (double)this.Value * (double)((Operand)rhs).Value;
			return oprResult;
		}

		public IOperand Divide(IOperand rhs)
		{
			if (!(rhs is DoubleOperand))
				throw new RPN_Exception("Argument invalid in DoubleOperand.Divide : rhs");
			DoubleOperand oprResult = new DoubleOperand("Result", Type.GetType("System.Int64"));
			oprResult.Value = (double)this.Value / (double)((Operand)rhs).Value;
			return oprResult;
		}

		public IOperand Modulo(IOperand rhs)
		{
			if (!(rhs is DoubleOperand))
				throw new RPN_Exception("Argument invalid in DoubleOperand.Modulo : rhs");
			DoubleOperand oprResult = new DoubleOperand("Result", Type.GetType("System.Int64"));
			oprResult.Value = (double)this.Value % (double)((Operand)rhs).Value;
			return oprResult;
		}

		public IOperand Not()
		{
			BoolOperand oprResult = new BoolOperand("Result", Type.GetType("System.Boolean"));

			switch (Convert.ToString(this.Value))
			{
				case "0":
					oprResult.Value = !(false);
					break;
				case "1":
					oprResult.Value = !(true);
					break;
				default:
					break;
			}

			return oprResult;
		}

		/// IComparisonOperators methods. Return values are always BooleanOperands type
		public IOperand EqualTo(IOperand rhs)
		{
			if (!(rhs is DoubleOperand))
				throw new RPN_Exception("Argument invalid in LongOperand.== : rhs");
			BoolOperand oprResult = new BoolOperand("Result");
			oprResult.Value = (double)this.Value == (double)((Operand)rhs).Value;
			return oprResult;
		}

		public IOperand NotEqualTo(IOperand rhs)
		{
			if (!(rhs is DoubleOperand))
				throw new RPN_Exception("Argument invalid in DoubleOperand.!= : rhs");
			BoolOperand oprResult = new BoolOperand("Result");
			oprResult.Value = ((double)this.Value != (double)((Operand)rhs).Value) ? true : false;
			return oprResult;
		}

		public IOperand LessThan(IOperand rhs)
		{
			if (!(rhs is DoubleOperand))
				throw new RPN_Exception("Argument invalid in DoubleOperand.< : rhs");
			BoolOperand oprResult = new BoolOperand("Result");
			oprResult.Value = ((double)this.Value < (double)((Operand)rhs).Value) ? true : false;
			return oprResult;
		}

		public IOperand LessThanOrEqualTo(IOperand rhs)
		{
			if (!(rhs is DoubleOperand))
				throw new RPN_Exception("Argument invalid in DoubleOperand.<= : rhs");
			BoolOperand oprResult = new BoolOperand("Result");
			oprResult.Value = ((double)this.Value <= (double)((Operand)rhs).Value) ? true : false;
			return oprResult;
		}

		public IOperand GreaterThan(IOperand rhs)
		{
			if (!(rhs is DoubleOperand))
				throw new RPN_Exception("Argument invalid in DoubleOperand.> : rhs");
			BoolOperand oprResult = new BoolOperand("Result");
			oprResult.Value = ((double)this.Value > (double)((Operand)rhs).Value) ? true : false;
			return oprResult;
		}

		public IOperand GreaterThanOrEqualTo(IOperand rhs)
		{
			if (!(rhs is DoubleOperand))
				throw new RPN_Exception("Argument invalid in DoubleOperand.>= : rhs");
			BoolOperand oprResult = new BoolOperand("Result");
			oprResult.Value = ((double)this.Value >= (double)((Operand)rhs).Value) ? true : false;
			return oprResult;
		}
	}

	///
	/// Operand corresponding to Boolean Type
	///
	public class BoolOperand : Operand, ILogicalOperations
	{
		public BoolOperand(string szVarName, object varValue)
			: base(szVarName, varValue)
		{
		}

		public BoolOperand(string szVarName)
			: base(szVarName)
		{
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}

		public override void ExtractAndSetValue(string szValue, bool bFormula)
		{
			m_VarValue = !bFormula ? Convert.ToBoolean(szValue) : false;
		}

		public IOperand AND(IOperand rhs)
		{
			if (!(rhs is BoolOperand))
				throw new RPN_Exception("Argument invalid in BoolOperand.&& : rhs");
			BoolOperand oprResult = new BoolOperand("Result");
			oprResult.Value = ((bool)this.Value && (bool)((Operand)rhs).Value) ? true : false;
			return oprResult;
		}

		public IOperand OR(IOperand rhs)
		{
			if (!(rhs is BoolOperand))
				throw new RPN_Exception("Argument invalid in BoolOperand.|| : rhs");
			BoolOperand oprResult = new BoolOperand("Result");
			oprResult.Value = ((bool)this.Value || (bool)((Operand)rhs).Value) ? true : false;
			return oprResult;
		}

		public IOperand Not()
		{
			BoolOperand oprResult = new BoolOperand("Result", Type.GetType("System.Boolean"));

			switch (Convert.ToString(this.Value).ToLower())
			{
				case "false":
					oprResult.Value = !(false);
					break;
				case "true":
					oprResult.Value = !(true);
					break;
				default:
					break;
			}

			return oprResult;
		}
	}

	public class OperandHelper
	{
		///
		/// Factory method to create corresponding Operands.
		/// Extended this method to create newer datatypes.
		///
		///
		///
		///
		///
		static public Operand CreateOperand(string szVarName, Type varType, object varValue)
		{
			Operand oprResult = null;
			switch (varType.ToString())
			{
				case "System.Int32":
				case "System.Int64":
					//oprResult = new LongOperand(szVarName, varValue);
					oprResult = new DoubleOperand(szVarName, varValue);
					return oprResult;
				//break;
				//case System.Decimal:
				//case System.Single:
				// oprResult = new DecimalOperand( szVarName, varValue );
				// return oprResult;
				// break;
				case "System.String":
					oprResult = new StringOperand(szVarName, varValue);
					return oprResult;
					//break;
			}

			throw new RPN_Exception("Unhandled type : " + varType.ToString());
		}

		static public Operand CreateOperand(string szVarName, Type varType)
		{
			return OperandHelper.CreateOperand(szVarName, varType, null);
		}
	}

	public class StringOperand : Operand, IComparisonOperations
	{
		public StringOperand(string szVarName, object varValue)
			: base(szVarName, varValue)
		{
		}

		public StringOperand(string szVarName)
			: base(szVarName)
		{
		}

		public override string ToString()
		{
			return m_szVarName;
		}

		public override void ExtractAndSetValue(string szValue, bool bFormula)
		{
			m_VarValue = !bFormula ? Convert.ToString(szValue) : "";
		}

		/// IComparisonOperators methods. Return values are always BooleanOperands type
		public IOperand EqualTo(IOperand rhs)
		{
			if (!(rhs is StringOperand))
				throw new RPN_Exception("Argument invalid in StringOperand.== : rhs");
			BoolOperand oprResult = new BoolOperand("Result");
			oprResult.Value = (string)this.Value == (string)((Operand)rhs).Value;
			return oprResult;
		}

		public IOperand NotEqualTo(IOperand rhs)
		{
			if (!(rhs is StringOperand))
				throw new RPN_Exception("Argument invalid in StringOperand.!= : rhs");
			BoolOperand oprResult = new BoolOperand("Result");
			oprResult.Value = ((string)this.Value != (string)((Operand)rhs).Value) ? true : false;
			return oprResult;
		}

		public IOperand LessThan(IOperand rhs)
		{
			// if( !(rhs is StringOperand) )
			throw new RPN_Exception("LessThan operator is invalid for string");
			// BoolOperand oprResult = new BoolOperand("Result");
			// oprResult.Value = ((string)this.Value < (string)((Operand)rhs).Value) ? true : false;
			// return oprResult;
		}

		public IOperand LessThanOrEqualTo(IOperand rhs)
		{
			// if( !(rhs is StringOperand) )
			throw new RPN_Exception("LessThanOrEqualTo operator is invalid for string");
			// BoolOperand oprResult = new BoolOperand("Result");
			// oprResult.Value = ((string)this.Value <= (string)((Operand)rhs).Value) ? true : false;
			// return oprResult;
		}

		public IOperand GreaterThan(IOperand rhs)
		{
			// if( !(rhs is StringOperand) )
			throw new RPN_Exception("GreaterThan operator is invalid for string");
			// BoolOperand oprResult = new BoolOperand("Result");
			// oprResult.Value = ((string)this.Value > (string)((Operand)rhs).Value) ? true : false;
			// return oprResult;
		}

		public IOperand GreaterThanOrEqualTo(IOperand rhs)
		{
			// if( !(rhs is StringOperand) )
			throw new RPN_Exception("GreaterThanOrEqualTo operator is invalid for string");
			// BoolOperand oprResult = new BoolOperand("Result");
			// oprResult.Value = ((string)this.Value >= (string)((Operand)rhs).Value) ? true : false;
			// return oprResult;
		}
	}

	#endregion

	#region Operators

	///
	/// Base class of all operators. Provides datastorage
	///
	public abstract class Operator : IOperator
	{
		public Operator(char cOperator)
		{
			m_szOperator = new String(cOperator, 1);
		}

		public Operator(string szOperator)
		{
			m_szOperator = szOperator;
		}

		public override string ToString()
		{
			return m_szOperator;
		}

		public abstract IOperand Eval(IOperand lhs, IOperand rhs);
		public abstract IOperand Eval(IOperand lhs);

		public string Value
		{
			get { return m_szOperator; }
			set { m_szOperator = value; }
		}

		protected string m_szOperator = "";
	}

	///
	/// Arithmetic Operator Class providing evaluation services for "+-/*%" operators.
	///
	public class ArithmeticOperator : Operator
	{
		public ArithmeticOperator(char cOperator)
			: base(cOperator)
		{
		}

		public ArithmeticOperator(string szOperator)
			: base(szOperator)
		{
		}
		//bool bBinaryOperator = true;

		public override IOperand Eval(IOperand lhs, IOperand rhs)
		{
			if (!(lhs is IArithmeticOperations))
				throw new RPN_Exception("Argument invalid in ArithmeticOperator.Eval - Invalid Expression : lhs");
			switch (m_szOperator)
			{
				case "+":
					return ((IArithmeticOperations)lhs).Plus(rhs);
				case "-":
					return ((IArithmeticOperations)lhs).Minus(rhs);
				case "*":
					return ((IArithmeticOperations)lhs).Multiply(rhs);
				case "/":
					return ((IArithmeticOperations)lhs).Divide(rhs);
				case "%":
					return ((IArithmeticOperations)lhs).Modulo(rhs);
			}

			throw new RPN_Exception("Unsupported Arithmetic operation " + m_szOperator);
		}

		public override IOperand Eval(IOperand lhs)
		{
			//if (!(lhs is IArithmeticOperations))
			//    throw new RPN_Exception("Argument invalid in ArithmeticOperator.Eval - Invalid Expression : lhs");
			if (lhs is IArithmeticOperations)
			{
				switch (m_szOperator)
				{
					case "~":
						return ((IArithmeticOperations)lhs).Negate();
					case "!":
						return ((IArithmeticOperations)lhs).Not();
				}
			}
			else if (lhs is BoolOperand)
			{
				switch (m_szOperator)
				{
					case "!":
						return ((BoolOperand)lhs).Not();
				}
			}

			throw new RPN_Exception("Unsupported Arithmetic operation " + m_szOperator);
		}
	}


	///
	/// Comparison Operator Class providing evaluation services for "==", "!=","<", "<=", ">", ">=" operators.
	///
	public class ComparisonOperator : Operator
	{
		public ComparisonOperator(char cOperator)
			: base(cOperator)
		{
		}

		public ComparisonOperator(string szOperator)
			: base(szOperator)
		{
		}

		//bool bBinaryOperator = true;
		public override IOperand Eval(IOperand lhs)
		{
			return null;
		}

		//{"==", "!=","<", "<=", ">", ">="}
		public override IOperand Eval(IOperand lhs, IOperand rhs)
		{
			if (!(lhs is IComparisonOperations))
				throw new RPN_Exception("Argument invalid in ComparisonOperator.Eval - Invalid Expression : lhs");
			switch (m_szOperator)
			{
				case "==":
					return ((IComparisonOperations)lhs).EqualTo(rhs);
				case "!=":
					return ((IComparisonOperations)lhs).NotEqualTo(rhs);
				case "<":
					return ((IComparisonOperations)lhs).LessThan(rhs);
				case "<=":
					return ((IComparisonOperations)lhs).LessThanOrEqualTo(rhs);
				case ">":
					return ((IComparisonOperations)lhs).GreaterThan(rhs);
				case ">=":
					return ((IComparisonOperations)lhs).GreaterThanOrEqualTo(rhs);
			}

			throw new RPN_Exception("Unsupported Comparison operation " + m_szOperator);
		}
	}

	///
	/// Logical Operator Class providing evaluation services for && and || operators.
	///
	public class LogicalOperator : Operator
	{
		public LogicalOperator(char cOperator)
			: base(cOperator)
		{
		}

		public LogicalOperator(string szOperator)
			: base(szOperator)
		{
		}
		//bool bBinaryOperator = true;

		public override IOperand Eval(IOperand lhs)
		{
			return null;
		}

		//{"&&", "||"}
		public override IOperand Eval(IOperand lhs, IOperand rhs)
		{
			if (!(lhs is ILogicalOperations))
				throw new RPN_Exception("Argument invalid in LogicalOperator.Eval - Invalid Expression : lhs");
			switch (m_szOperator)
			{
				case "&&":
					return ((ILogicalOperations)lhs).AND(rhs);
				case "||":
					return ((ILogicalOperations)lhs).OR(rhs);
			}

			throw new RPN_Exception("Unsupported Logical operation " + m_szOperator);
		}
	}

	public class OperatorHelper
	{
		///
		/// Factory method to create Operator objects.
		///
		///
		///
		static public IOperator CreateOperator(string szOperator)
		{
			IOperator oprtr = null;
			if (OperatorHelper.IsArithmeticOperator(szOperator))
			{
				oprtr = new ArithmeticOperator(szOperator);
				return oprtr;
			}

			if (OperatorHelper.IsComparisonOperator(szOperator))
			{
				oprtr = new ComparisonOperator(szOperator);
				return oprtr;
			}

			if (OperatorHelper.IsLogicalOperator(szOperator))
			{
				oprtr = new LogicalOperator(szOperator);
				return oprtr;
			}

			throw new RPN_Exception("Unhandled Operator : " + szOperator);
		}

		static public IOperator CreateOperator(char cOperator)
		{
			return CreateOperator(new string(cOperator, 1));
		}

		/// Some helper functions.
		public static bool IsOperator(string currentOp)
		{
			int nPos = Array.IndexOf(m_AllOps, currentOp.Trim());
			if (nPos != -1)
				return true;
			else
				return false;
		}

		public static bool IsUnaryOperator(string currentOp)
		{
			int nPos = Array.IndexOf(m_AllUnaryOps, currentOp.Trim());
			if (nPos != -1)
				return true;
			else
				return false;
		}

		public static bool IsArithmeticOperator(string currentOp)
		{
			int nPos = Array.IndexOf(m_AllArithmeticOps, currentOp);
			if (nPos != -1)
				return true;
			else
				return false;
		}

		public static bool IsComparisonOperator(string currentOp)
		{
			int nPos = Array.IndexOf(m_AllComparisonOps, currentOp);
			if (nPos != -1)
				return true;
			else
				return false;
		}

		public static bool IsLogicalOperator(string currentOp)
		{
			int nPos = Array.IndexOf(m_AllLogicalOps, currentOp);
			if (nPos != -1)
				return true;
			else
				return false;
		}

		#region Precedence

		/// Precedence is determined by relative indices of the operators defined in
		/// in m_AllOps variable
		///
		/// Summary of IsLowerPrecOperator.
		///
		///
		///
		///
		///
		public static bool IsLowerPrecOperator(string currentOp, string prevOp)
		{
			int nCurrIdx;
			int nPrevIdx;
			GetCurrentAndPreviousIndex(m_AllOps, currentOp, prevOp, out nCurrIdx, out nPrevIdx);
			if (nCurrIdx < nPrevIdx)
			{
				return true;
			}

			return false;
		}

		///
		/// Summary of IsHigherPrecOperator.
		///
		///
		///
		///
		///
		public static bool IsHigherPrecOperator(string currentOp, string prevOp)
		{
			int nCurrIdx;
			int nPrevIdx;
			GetCurrentAndPreviousIndex(m_AllOps, currentOp, prevOp, out nCurrIdx, out nPrevIdx);
			if (nCurrIdx > nPrevIdx)
			{
				return true;
			}

			return false;
		}

		///
		/// Summary of IsEqualPrecOperator.
		///
		///
		///
		///
		///
		public static bool IsEqualPrecOperator(string currentOp, string prevOp)
		{
			int nCurrIdx;
			int nPrevIdx;
			GetCurrentAndPreviousIndex(m_AllOps, currentOp, prevOp, out nCurrIdx, out nPrevIdx);
			if (nCurrIdx == nPrevIdx)
			{
				return true;
			}

			return false;
		}

		///
		/// Summary of GetCurrentAndPreviousIndex.
		///
		///
		///
		///
		///
		///
		///
		private static void GetCurrentAndPreviousIndex(string[] allOps, string currentOp, string prevOp,
			out int nCurrIdx, out int nPrevIdx)
		{
			nCurrIdx = -1;
			nPrevIdx = -1;
			for (int nIdx = 0; nIdx < allOps.Length; nIdx++)
			{
				if (allOps[nIdx] == currentOp)
				{
					nCurrIdx = nIdx;
				}

				if (allOps[nIdx] == prevOp)
				{
					nPrevIdx = nIdx;
				}

				if (nPrevIdx != -1 && nCurrIdx != -1)
				{
					break;
				}
			}

			if (nCurrIdx == -1)
			{
				throw new RPN_Exception("Unknown operator - " + currentOp);
			}

			if (nPrevIdx == -1)
			{
				throw new RPN_Exception("Unknown operator - " + prevOp);
			}
		}

		#endregion

		#region RegEx

		///
		/// This gets the regular expression used to find operators in the input
		/// expression.
		///
		///
		///
		static public string GetOperatorsRegEx(ExpressionType exType)
		{
			StringBuilder strRegex = new StringBuilder();
			if ((exType & ExpressionType.ET_ARITHMETIC).Equals(ExpressionType.ET_ARITHMETIC))
			{
				if (strRegex.Length == 0)
				{
					strRegex.Append(m_szArthmtcRegEx);
				}
				else
				{
					strRegex.Append("|" + m_szArthmtcRegEx);
				}
			}

			if ((exType & ExpressionType.ET_COMPARISON).Equals(ExpressionType.ET_COMPARISON))
			{
				if (strRegex.Length == 0)
				{
					strRegex.Append(m_szCmprsnRegEx);
				}
				else
				{
					strRegex.Append("|" + m_szCmprsnRegEx);
				}
			}

			if ((exType & ExpressionType.ET_LOGICAL).Equals(ExpressionType.ET_LOGICAL))
			{
				if (strRegex.Length == 0)
				{
					strRegex.Append(m_szLgclRegEx);
				}
				else
				{
					strRegex.Append("|" + m_szLgclRegEx);
				}
			}

			if (strRegex.Length == 0)
				throw new RPN_Exception("Invalid combination of ExpressionType value");
			return "(" + strRegex.ToString() + ")";
		}

		///
		/// Expression to pattern match various operators
		///
		static string m_szArthmtcRegEx = @"[+\-*/%()]{1}";

		static string m_szCmprsnRegEx = @"[=<>!]{1,2}";

		static string m_szLgclRegEx = @"[&|]{2}";
		//static string m_szUnaryRegEx = @"[~]{1}";

		#endregion

		public static string[] AllOperators
		{
			get { return m_AllOps; }
		}

		///
		/// All Operators supported by this module currently.
		/// Modify here to add more operators IN ACCORDANCE WITH their precedence.
		/// Additionally add into individual variables to support some helper methods above.
		///
		static string[] m_AllOps =
		{
			"||", "&&", "|", "^", "&", "==", "!=",
			"<", "<=", ">", ">=", "+", "-", "*", "/", "~", "%", "(", ")", "!"
		};

		static string[] m_AllArithmeticOps = { "+", "-", "~", "*", "/", "%", "!" };
		static string[] m_AllComparisonOps = { "==", "!=", "<", "<=", ">", ">=" };
		static string[] m_AllLogicalOps = { "&&", "||" };
		static string[] m_AllUnaryOps = { "~", "!" };
	}

	#endregion

	#region TODO List

	/// TODO: Support for unary operators
	/// TODO: Support for bitwise & and |
	/// TODO: how to handle a combo expression with multiple braces as a logical/comparison expression?
	/// e.g. ((2+3)*2<10 || 1!=1) && 2*2==4 as a logical expression?
	/// TODO: Form to accept values for formulae

	#endregion

	// Function to test for Positive Integers.

	#region Content check

	public class StringContentCheck
	{
		static public bool IsNaturalNumber(String strNumber)
		{
			Regex objNotNaturalPattern = new Regex("[^0-9]");
			Regex objNaturalPattern = new Regex("0*[1-9][0-9]*");
			return !objNotNaturalPattern.IsMatch(strNumber) &&
				   objNaturalPattern.IsMatch(strNumber);
		}

		// Function to test for Positive Integers with zero inclusive

		static public bool IsWholeNumber(String strNumber)
		{
			Regex objNotWholePattern = new Regex("[^0-9]");
			return !objNotWholePattern.IsMatch(strNumber);
		}

		// Function to Test for Integers both Positive & Negative

		static public bool IsInteger(String strNumber)
		{
			Regex objNotIntPattern = new Regex("[^0-9-]");
			Regex objIntPattern = new Regex("^-[0-9]+$|^[0-9]+$");
			return !objNotIntPattern.IsMatch(strNumber) && objIntPattern.IsMatch(strNumber);
		}

		// Function to Test for Positive Number both Integer & Real

		static public bool IsPositiveNumber(String strNumber)
		{
			Regex objNotPositivePattern = new Regex("[^0-9.]");
			Regex objPositivePattern = new Regex("^[.][0-9]+$|[0-9]*[.]*[0-9]+$");
			Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
			return !objNotPositivePattern.IsMatch(strNumber) &&
				   objPositivePattern.IsMatch(strNumber) &&
				   !objTwoDotPattern.IsMatch(strNumber);
		}

		// Function to test whether the string is valid number or not
		static public bool IsNumber(String strNumber)
		{
			Regex objNotNumberPattern = new Regex("[^0-9.-]");
			Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
			Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
			String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
			String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
			Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");
			return !objNotNumberPattern.IsMatch(strNumber) &&
				   !objTwoDotPattern.IsMatch(strNumber) &&
				   !objTwoMinusPattern.IsMatch(strNumber) &&
				   objNumberPattern.IsMatch(strNumber);
		}

		// Function To test for Alphabets.

		static public bool IsAlpha(String strToCheck)
		{
			Regex objAlphaPattern = new Regex("[^a-zA-Z ]");
			return !objAlphaPattern.IsMatch(strToCheck);
		}

		// Function to Check for AlphaNumeric.
		static public bool IsAlphaNumeric(String strToCheck)
		{
			Regex objAlphaNumericPattern = new Regex("[^a-zA-Z0-9 ]");
			return !objAlphaNumericPattern.IsMatch(strToCheck);
		}
	}
}

#endregion