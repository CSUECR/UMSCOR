using System;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace HOHO18.Common.Web
{
	/// <summary>
	/// ҳ������У����
	/// HOHO18
	/// 2008.10
	/// </summary>
    public sealed class PageValidate
	{
        private static readonly string required = ".+";
        private static readonly string emp = "^[\\s| ]*$";
        private static readonly string phone = "^((\\(\\d{2,3}\\))|(\\d{3}\\-))?(\\(0\\d{2,3}\\)|0\\d{2,3}-)?[1-9]\\d{6,7}(\\-\\d{1,4})?$";
        private static readonly string mobile = "^((\\(\\d{2,3}\\))|(\\d{3}\\-))?((13\\d{9})|(15[389]\\d{8}))$";
        private static readonly string number = "^\\d+$";
        private static readonly string qq = "^\\d{5,10}$";
        private static readonly string chinaname = "^[\u4e00-\u9fa5]{0,5}$";
        private static readonly string username = "^.{4,16}$";  
        private static readonly string email ="^\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$";

		private static Regex RegNumber = new Regex("^[0-9]+$");
		private static Regex RegNumberSign = new Regex("^[+-]?[0-9]+$");
		private static Regex RegDecimal = new Regex("^[0-9]+[.]?[0-9]+$");
		private static Regex RegDecimalSign = new Regex("^[+-]?[0-9]+[.]?[0-9]+$"); //�ȼ���^[+-]?\d+[.]?\d+$
		private static Regex RegEmail = new Regex("^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$");//w Ӣ����ĸ�����ֵ��ַ������� [a-zA-Z0-9] �﷨һ�� 
		private static Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");

        public PageValidate()
		{
		}


		#region �����ַ������		
		
		/// <summary>
		/// ���Request��ѯ�ַ����ļ�ֵ���Ƿ������֣���󳤶�����
		/// </summary>
		/// <param name="req">Request</param>
		/// <param name="inputKey">Request�ļ�ֵ</param>
		/// <param name="maxLen">��󳤶�</param>
		/// <returns>����Request��ѯ�ַ���</returns>
		public static string FetchInputDigit(HttpRequest req, string inputKey, int maxLen)
		{
			string retVal = string.Empty;
			if(inputKey != null && inputKey != string.Empty)
			{
				retVal = req.QueryString[inputKey];
				if(null == retVal)
					retVal = req.Form[inputKey];
				if(null != retVal)
				{
					retVal = SqlText(retVal, maxLen);
					if(!IsNumber(retVal))
						retVal = string.Empty;
				}
			}
			if(retVal == null)
				retVal = string.Empty;
			return retVal;
		}		
		/// <summary>
		/// �Ƿ������ַ���
		/// </summary>
		/// <param name="inputData">�����ַ���</param>
		/// <returns></returns>
		public static bool IsNumber(string inputData)
		{
			Match m = RegNumber.Match(inputData);
			return m.Success;
		}		
		/// <summary>
		/// �Ƿ������ַ��� �ɴ�������
		/// </summary>
		/// <param name="inputData">�����ַ���</param>
		/// <returns></returns>
		public static bool IsNumberSign(string inputData)
		{
			Match m = RegNumberSign.Match(inputData);
			return m.Success;
		}		
		/// <summary>
		/// �Ƿ��Ǹ�����
		/// </summary>
		/// <param name="inputData">�����ַ���</param>
		/// <returns></returns>
		public static bool IsDecimal(string inputData)
		{
			Match m = RegDecimal.Match(inputData);
			return m.Success;
		}		
		/// <summary>
		/// �Ƿ��Ǹ����� �ɴ�������
		/// </summary>
		/// <param name="inputData">�����ַ���</param>
		/// <returns></returns>
		public static bool IsDecimalSign(string inputData)
		{
			Match m = RegDecimalSign.Match(inputData);
			return m.Success;
		}		

		#endregion

		#region ���ļ��

		/// <summary>
		/// ����Ƿ��������ַ�
		/// </summary>
		/// <param name="inputData"></param>
		/// <returns></returns>
		public static bool IsHasCHZN(string inputData)
		{
			Match m = RegCHZN.Match(inputData);
			return m.Success;
		}	

		#endregion

		#region �ʼ���ַ
		/// <summary>
		/// �Ƿ��ʼ� �ɴ�������
		/// </summary>
		/// <param name="inputData">�����ַ���</param>
		/// <returns></returns>
		public static bool IsEmail(string inputData)
		{
			Match m = RegEmail.Match(inputData);
			return m.Success;
		}		

		#endregion



        #region ����

        /// <summary>
		/// ����ַ�����󳤶ȣ�����ָ�����ȵĴ�
		/// </summary>
		/// <param name="sqlInput">�����ַ���</param>
		/// <param name="maxLength">��󳤶�</param>
		/// <returns></returns>			
		public static string SqlText(string sqlInput, int maxLength)
		{			
			if(sqlInput != null && sqlInput != string.Empty)
			{
				sqlInput = sqlInput.Trim();							
				if(sqlInput.Length > maxLength)//����󳤶Ƚ�ȡ�ַ���
					sqlInput = sqlInput.Substring(0, maxLength);
			}
			return sqlInput;
		}		
		/// <summary>
		/// �ַ�������
		/// </summary>
		/// <param name="inputData"></param>
		/// <returns></returns>
		public static string HtmlEncode(string inputData)
		{
			return HttpUtility.HtmlEncode(inputData);
		}
		/// <summary>
		/// ����Label��ʾEncode���ַ���
		/// </summary>
		/// <param name="lbl"></param>
		/// <param name="txtInput"></param>
		public static void SetLabel(Label lbl, string txtInput)
		{
			lbl.Text = HtmlEncode(txtInput);
		}
		public static void SetLabel(Label lbl, object inputObj)
		{
			SetLabel(lbl, inputObj.ToString());
		}		
		//�ַ�������
		public static string InputText(string inputString, int maxLength) 
		{			
			StringBuilder retVal = new StringBuilder();

			// ����Ƿ�Ϊ��
			if ((inputString != null) && (inputString != String.Empty)) 
			{
				inputString = inputString.Trim();
				
				//��鳤��
				if (inputString.Length > maxLength)
					inputString = inputString.Substring(0, maxLength);
				
				//�滻Σ���ַ�
				for (int i = 0; i < inputString.Length; i++) 
				{
					switch (inputString[i]) 
					{
						case '"':
							retVal.Append("&quot;");
							break;
						case '<':
							retVal.Append("&lt;");
							break;
						case '>':
							retVal.Append("&gt;");
							break;
						default:
							retVal.Append(inputString[i]);
							break;
					}
				}				
				retVal.Replace("'", " ");// �滻������
			}
			return retVal.ToString();
			
		}
		/// <summary>
		/// ת���� HTML code
		/// </summary>
		/// <param name="str">string</param>
		/// <returns>string</returns>
		public static string Encode(string str)
		{			
			str = str.Replace("&","&amp;");
			str = str.Replace("'","''");
			str = str.Replace("\"","&quot;");
			str = str.Replace(" ","&nbsp;");
			str = str.Replace("<","&lt;");
			str = str.Replace(">","&gt;");
			str = str.Replace("\n","<br>");
			return str;
		}
		/// <summary>
		///����html�� ��ͨ�ı�
		/// </summary>
		/// <param name="str">string</param>
		/// <returns>string</returns>
		public static string Decode(string str)
		{			
			str = str.Replace("<br>","\n");
			str = str.Replace("&gt;",">");
			str = str.Replace("&lt;","<");
			str = str.Replace("&nbsp;"," ");
			str = str.Replace("&quot;","\"");
			return str;
		}

		#endregion

        #region �Լ���

        /// <summary>
        /// ������֤�û���
        /// </summary>
        /// <param name="data">����֤����</param>
        /// <returns>0-�Ϸ�,1���Ϸ�</returns>        
        public static int ValidateUsername(string data)
        {
            if (Regex.IsMatch(data, username))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// ������֤����
        /// </summary>
        /// <param name="data">����֤����</param>
        /// <returns>0-�Ϸ�</returns>        
        public static int ValidatePassword(string data)
        {
            if (data.Length < 6 || data.Length > 16)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// ������֤����,����Ϊ�յ�״��
        /// </summary>
        /// <param name="data">����֤����</param>
        /// <returns>0-�Ϸ�,1-���Ϸ�</returns>        
        public static int ValidateUpPassword(string data)
        {
            if (String.IsNullOrEmpty(data))
            {
                return 0;
            }
            if (data.Length < 6 || data.Length > 16)
            {
                return 1;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// ������֤��������������Ƿ���ͬ
        /// </summary>
        /// <param name="data">����֤����</param>
        /// <param name="data">�Ƚ�����</param>
        /// <returns>0-һ�£�1-��һ��</returns>        
        public static int ValidateCheckPassword(string data, string checkData)
        {
            if (String.Compare(data, checkData) == 0)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// ���ڷ������֤������ʾ�Ƿ�Ϸ�
        /// </summary>
        /// <param name="data">����֤����</param>
        /// <returns>0-�Ϸ���1-���Ϸ�</returns>        
        public static int ValidatePsdQuestion(string data)
        {
            if (Regex.IsMatch(data, required))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// ���ڷ������֤���Ƿ�Ϸ�
        /// </summary>
        /// <param name="data">����֤����</param>
        /// <returns>0-�Ϸ���1-���Ϸ�</returns>        
        public static int ValidatePsdAnswer(string data)
        {
            if (data.Length < 6 || data.Length > 30)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        } 

        

        /// <summary>
        /// ���ڷ������֤�����Ƿ�Ϸ�
        /// </summary>
        /// <param name="data">����֤����</param>
        /// <returns>0-�Ϸ���1-���Ϸ�</returns>       
        public static int ValidateName(string data)
        {
            if (Regex.IsMatch(data, chinaname))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// ���ڷ������֤��ϵ�绰�Ƿ�Ϸ�,����Ϊ�յĵ绰
        /// </summary>
        /// <param name="data">����֤����</param>
        /// <returns>0-�Ϸ���1-���Ϸ�</returns>       
        public static int ValidateTel(string data)
        {
            if (Regex.IsMatch(data, emp) || Regex.IsMatch(data, phone) || Regex.IsMatch(data, mobile))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// ���ڷ������֤���������Ƿ�Ϸ�,����Ϊ�յĵ�������
        /// </summary>
        /// <param name="data">����֤����</param>
        /// <returns>0-�Ϸ���1-���Ϸ�</returns>       
        public static int ValidateEmail(string data)
        {
            if (Regex.IsMatch(data, emp) || Regex.IsMatch(data, email))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// ���ڷ������֤QQ�Ƿ�Ϸ�
        /// </summary>
        /// <param name="data">����֤����</param>
        /// <returns>0-�Ϸ���1-���Ϸ�</returns>        
        public static int ValidateQQ(string data)
        {
            if (Regex.IsMatch(data, qq) || Regex.IsMatch(data, emp))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// ���ڷ������֤�ǿ�����
        /// </summary>
        /// <param name="data">����֤����</param>
        /// <returns>0-�Ϸ���1-���Ϸ�</returns>      
        public static int ValidateRequire(string data)
        {
            if (Regex.IsMatch(data, required))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// ���ڷ������֤����
        /// </summary>
        /// <param name="data">����֤����</param>
        /// <returns>0-�Ϸ���1-���Ϸ�</returns>      
        public static int ValidateNumber(string data)
        {
            if (Regex.IsMatch(data, number))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// �����Զ�����֤
        /// </summary>
        /// <param name="data">����֤����</param>
        /// <returns>0-�Ϸ���1-���Ϸ�</returns>       
        public static int ValidateCustom(string data, string regularExpression)
        {
            string regularExp = regularExpression;
            if (Regex.IsMatch(data, regularExp))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        #endregion
    }
}
