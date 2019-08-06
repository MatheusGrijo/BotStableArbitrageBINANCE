using System;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography;

public static class Http
{
    static string ByteToString(byte[] buff)
    {
        string sbinary = "";
        for (int i = 0; i < buff.Length; i++)
        {
            sbinary += buff[i].ToString("X2"); /* hex format */
        }
        return (sbinary);
    }

    public static readonly Object objLock = new object();
    public static readonly Object objLock2 = new object();
    public static string post(String url, String parameters, bool fast = false)
    {
        string _ret = null;
        while (_ret == null)
        {
            _ret = _post(url, parameters);
            if (_ret == null)
                System.Threading.Thread.Sleep(new Random().Next(1000, 1200));

        }
        return _ret;
    }

    public static string _post(String url, String parameters, bool fast = false)
    {
        try
        {
            // lock (objLock)
            {


                Logger.log(url + parameters);
                var request = (HttpWebRequest)WebRequest.Create(url);
                //System.Threading.Thread.Sleep(1000);
                parameters = "nonce=" + (decimal.Parse(DateTime.Now.ToString("yyyyMMddHHmmssfffff"))) + "&" + parameters;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var data = Encoding.ASCII.GetBytes(parameters);

                HMACSHA512 encryptor = new HMACSHA512();
                encryptor.Key = Encoding.ASCII.GetBytes(Key.getSecret());
                String sign = ByteToString(encryptor.ComputeHash(data));

                request.Headers["Key"] = Key.getKey();
                request.Headers["Sign"] = sign;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                String result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                
                Logger.log(result);
                System.Threading.Thread.Sleep(new Random().Next(2000, 3000));
                return result;
            }
        }
        catch (Exception ex)
        {
            Logger.log("ERROR POST " + ex.Message + ex.StackTrace);
            return null;
        }
        finally
        {

        }
    }


    public static string postGeneric(String url, String parameters)
    {
        try
        {
            // lock (objLock)
            {
                System.Threading.Thread.Sleep(new Random().Next(2000, 3500));
                Logger.log(url + parameters);
                var request = (HttpWebRequest)WebRequest.Create(url);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var data = Encoding.ASCII.GetBytes(parameters);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                String result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                
                Logger.log(result);                
                return result;
            }
        }
        catch (Exception ex)
        {
            Logger.log("ERROR POST " + ex.Message + ex.StackTrace);
            return null;
        }
        finally
        {

        }
    }

    public static string GerarHashMd5(string input)
    {
        MD5 md5Hash = MD5.Create();
        // Converter a String para array de bytes, que é como a biblioteca trabalha.
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        // Cria-se um StringBuilder para recompôr a string.
        StringBuilder sBuilder = new StringBuilder();

        // Loop para formatar cada byte como uma String em hexadecimal
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        return sBuilder.ToString();
    }

    public static string getCache(String url, bool wait = true)
    {
        try
        {
            String aux = GerarHashMd5(url);

            if (System.IO.File.Exists(Program.location + @"\cache\" + aux + ".txt"))            
                return System.IO.File.ReadAllText(Program.location + @"\cache\" + aux + ".txt");

            Console.WriteLine("Wait 6s...");
            System.Threading.Thread.Sleep(new Random().Next(5000, 6000));


            String r = "";
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
            httpWebRequest.Method = "GET";
            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var responseStream = httpWebResponse.GetResponseStream();
            if (responseStream != null)
            {
                var streamReader = new StreamReader(responseStream);
                r = streamReader.ReadToEnd();
            }
            if (responseStream != null) responseStream.Close();
            //Console.WriteLine(r);

            System.IO.StreamWriter w = new StreamWriter(Program.location + @"\cache\" + aux + ".txt",true);
            w.Write(r);
            w.Close();
            w.Dispose();

            return r;
        }
        catch (WebException ex)
        {
            return null;
        }
    }

    public static string get(String url,bool wait = true)
    {



        try
        {
          
            String r = "";
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
            httpWebRequest.Method = "GET";
            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var responseStream = httpWebResponse.GetResponseStream();
            if (responseStream != null)
            {
                var streamReader = new StreamReader(responseStream);
                r = streamReader.ReadToEnd();
            }
            if (responseStream != null) responseStream.Close();
            //Console.WriteLine(r);
            return r;
        }
        catch (WebException ex)
        {
            return null;
        }
    }

}