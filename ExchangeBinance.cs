using Binance.Net;
using Binance.Net.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class ExchangeBinance
{

    public static int volumeDay = 800;
    static JContainer exchangeInfo = null;

    public ExchangeBinance()
    {
        try
        {
         

            String json = Http.get("https://api.binance.com/api/v1/exchangeInfo");
            exchangeInfo = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));

        }
        catch
        {

        }
    }


    public class KLine
    {
        public string symbol;
        public BinanceStreamKlineData kline;

    }

    public static List<KLine> lstKline = new List<KLine>();
    

    public class Ticker
    {
        public string symbol;
        public BinanceStreamTick tick;

    }

    public static List<Ticker> lstTicker = new List<Ticker>();
    


    
    public decimal getLastPriceCACHE(string pair,string cache)
    {


        String jsonTicker = cache;
        JContainer jCointanerTicker = (JContainer)JsonConvert.DeserializeObject(jsonTicker, (typeof(JContainer)));
        foreach (var itemTicker in jCointanerTicker)
            if (pair == itemTicker["symbol"].ToString())
            {
                Console.WriteLine(pair + ":" + decimal.Parse(itemTicker["lastPrice"].ToString().ToString().Replace(".", ",")));
                return decimal.Parse(itemTicker["lastPrice"].ToString().ToString().Replace(".", ","));
            }

        return 0;
    }

    public decimal getLastPrice(string pair)
    {
    

        System.Threading.Thread.Sleep(1000);
        String jsonTicker = Http.get("https://api.binance.com/api/v1/ticker/24hr");
        JContainer jCointanerTicker = (JContainer)JsonConvert.DeserializeObject(jsonTicker, (typeof(JContainer)));
        foreach (var itemTicker in jCointanerTicker)
            if (pair == itemTicker["symbol"].ToString())
                return decimal.Parse(itemTicker["lastPrice"].ToString().ToString().Replace(".", ","));

        return 0;
    }
    



    public string getName()
    {
        return "BINANCE";
    }


    public void loadBalances()
    {
        throw new NotImplementedException();
    }


    Object objLock = new Object();
    public string post(String url, String parameters, Method method = Method.POST)
    {
        try
        {
            lock (objLock)
            {


                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient("https://api.binance.com");

                HMACSHA256 encryptor = new HMACSHA256();
                encryptor.Key = Encoding.ASCII.GetBytes(Key.getSecret());
                String sign = BitConverter.ToString(encryptor.ComputeHash(Encoding.ASCII.GetBytes(parameters))).Replace("-", "");
                parameters += "&signature=" + sign;

                var request = new RestRequest("/api/v3/order?" + parameters, method);
                request.AddHeader("X-MBX-APIKEY", Key.getKey());
                var response = client.Execute(request);
                Logger.log(response.Content);

                
                return response.Content.ToString();
            }
        }
        catch (Exception ex)
        {
            
            return null;
        }
        finally
        {
        }
    }

    public string httppost(String url, String parameters, String key, String secret)
    {
        try
        {
            // lock (objLock)
            {



                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient("https://api.binance.com");

                HMACSHA256 encryptor = new HMACSHA256();
                encryptor.Key = Encoding.ASCII.GetBytes(Key.getSecret());
                String sign = BitConverter.ToString(encryptor.ComputeHash(Encoding.ASCII.GetBytes(parameters))).Replace("-", "");
                parameters += "&signature=" + sign;

                var request = new RestRequest(url + "?" + parameters, Method.POST);
                request.AddHeader("X-MBX-APIKEY", Key.getKey());
                var response = client.Execute(request);
                Console.WriteLine(response.Content);
                return response.Content.ToString();
            }
        }
        catch (Exception ex)
        {            
            return null;
        }
        finally
        {
        }
    }


    public string order(string type, string pair, decimal amount, decimal price, bool lockQuantity = true, string timeInForce = "GTC")
    {
     

        amount = Math.Round(amount / getQuantity(pair)) * getQuantity(pair);
        amount = Math.Round(amount, getQuotePrecision(pair));
        amount -= getQuantity(pair);

        price = Math.Round(price / getPriceFilter(pair)) * getPriceFilter(pair);
        price = Math.Round(price, getPrecision(pair));


        price = Math.Round(price, 8);
        amount = Math.Round(amount, 8);

        int countTry = 0;
        string ret = post("https://api.binance.com/api/v3/order", "symbol=" + pair.ToUpper() + "&side=" + type.ToUpper() + "&type=LIMIT&timeInForce=" + timeInForce + "&quantity=" + amount.ToString().Replace(",", ".") + "&price=" + price.ToString().Replace(",", ".") + "&timestamp=" + Utils.GenerateTimeStamp(DateTime.Now.ToUniversalTime()), Method.POST);
        while (ret.IndexOf("insufficient balance") >= 0)
        {
            for (int i = 0; i < 100; i++)
            {
                amount -= 1;
            }
            

            ret = post("https://api.binance.com/api/v3/order", "symbol=" + pair.ToUpper() + "&side=" + type.ToUpper() + "&type=LIMIT&timeInForce=" + timeInForce + "&quantity=" + amount.ToString().Replace(",", ".") + "&price=" + price.ToString().Replace(",", ".") + "&timestamp=" + Utils.GenerateTimeStamp(DateTime.Now.ToUniversalTime()), Method.POST);
            countTry++;
            if (countTry > 100)
            {
              
                amount = Math.Round(amount / getQuantity(pair)) * getQuantity(pair);
                amount = Math.Round(amount, getQuotePrecision(pair));
                amount -= getQuantity(pair);
                amount = Math.Round(amount, 8);
                ret = post("https://api.binance.com/api/v3/order", "symbol=" + pair.ToUpper() + "&side=" + type.ToUpper() + "&type=LIMIT&timeInForce=" + timeInForce + "&quantity=" + amount.ToString().Replace(",", ".") + "&price=" + price.ToString().Replace(",", ".") + "&timestamp=" + Utils.GenerateTimeStamp(DateTime.Now.ToUniversalTime()), Method.POST);
                return ret;
            }
            System.Threading.Thread.Sleep(new Random().Next(300, 500));
        }
        return ret;
    }
    public string orderMarket(string type, string pair, decimal amount)
    {


        amount = Math.Round(amount / getQuantity(pair)) * getQuantity(pair);
        amount = Math.Round(amount, getQuotePrecision(pair));



        int countTry = 0;
        string ret = post("https://api.binance.com/api/v3/order", "symbol=" + pair.ToUpper() + "&side=" + type.ToUpper() + "&type=MARKET&quantity=" + amount.ToString().Replace(",", ".") + "&timestamp=" + Utils.GenerateTimeStamp(DateTime.Now.ToUniversalTime()), Method.POST);
        decimal amountDecrease = 3;
        while (ret.IndexOf("insufficient balance") >= 0 || ret.IndexOf("many new orders") >= 0 || ret.IndexOf("LOT_SIZE") >= 0)
        {
            for (int i = 0; i < 2; i++)
                amount -= getQuantity(pair);
            amountDecrease++;
            ret = post("https://api.binance.com/api/v3/order", "symbol=" + pair.ToUpper() + "&side=" + type.ToUpper() + "&type=MARKET&quantity=" + amount.ToString().Replace(",", ".") + "&timestamp=" + Utils.GenerateTimeStamp(DateTime.Now.ToUniversalTime()), Method.POST);
            countTry++;
            if (countTry > 900)
            {
               
                amount = Math.Round(amount / getQuantity(pair)) * getQuantity(pair);
                amount = Math.Round(amount, getQuotePrecision(pair));
                amount -= getQuantity(pair);
                amount = Math.Round(amount, getQuotePrecision(pair));
                ret = post("https://api.binance.com/api/v3/order", "symbol=" + pair.ToUpper() + "&side=" + type.ToUpper() + "&type=MARKET&quantity=" + amount.ToString().Replace(",", ".") + "&timestamp=" + Utils.GenerateTimeStamp(DateTime.Now.ToUniversalTime()), Method.POST);
                return ret;
            }
        }
        return ret;

    }
    

    public static decimal getQuantity(String coin)
    {

        foreach (var item in exchangeInfo["symbols"])
        {
            if (item["symbol"].ToString().Trim().ToUpper() == coin.Trim().ToUpper())
            {
                return decimal.Parse(item["filters"][2]["stepSize"].ToString().Replace(".", ","));
            }
        }

        return 1;
    }

    public static decimal getPriceFilter(String coin)
    {

        foreach (var item in exchangeInfo["symbols"])
        {
            if (item["symbol"].ToString().Trim().ToUpper() == coin.Trim().ToUpper())
            {
                return decimal.Parse(item["filters"][0]["tickSize"].ToString().Replace(".", ","));
            }
        }

        return 1;
    }



    public static int getPrecision(String coin)
    {

        foreach (var item in exchangeInfo["symbols"])
        {
            if (item["symbol"].ToString().Trim().ToUpper() == coin.Trim().ToUpper())
            {
                return int.Parse(item["baseAssetPrecision"].ToString().Replace(".", ","));
            }
        }

        return 1;
    }

    public static int getQuotePrecision(String coin)
    {

        foreach (var item in exchangeInfo["symbols"])
        {
            if (item["symbol"].ToString().Trim().ToUpper() == coin.Trim().ToUpper())
            {
                return int.Parse(item["quotePrecision"].ToString().Replace(".", ","));
            }
        }

        return 1;
    }

    public string getBalancesJSON()
    {
        try
        {
            var client = new RestClient("https://api.binance.com");
            String parameters = "timestamp=" + Utils.GenerateTimeStamp(DateTime.Now.ToUniversalTime()).Split(',')[0];
            HMACSHA256 encryptor = new HMACSHA256();
            encryptor.Key = Encoding.ASCII.GetBytes(Key.getSecret());
            String sign = BitConverter.ToString(encryptor.ComputeHash(Encoding.ASCII.GetBytes(parameters))).Replace("-", "");
            parameters += "&signature=" + sign;
            var request = new RestRequest("/api/v3/account?" + parameters, Method.GET);
            request.AddHeader("X-MBX-APIKEY", Key.getKey());
            var response = client.Execute(request);
            //Console.WriteLine(response.Content);
            return  response.Content.ToString();
            
        }
        catch (Exception ex)
        {
            return "";
        }
        finally
        {
        }
    }

    public decimal getBalances(string _coin, string json)
    {
        try
        {
          
            JContainer dt = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));


            foreach (var item in dt["balances"])
            {
                String coin = item["asset"].ToString();
                decimal value = decimal.Parse(item["free"].ToString().Replace(".", ","));

                if (value > 0)
                {
                    if (coin.ToString().ToUpper() == _coin.ToUpper())
                        return value;
                }
            }
            return 0;
        }
        catch (Exception ex)
        {
            return 0;
        }
        finally
        {
        }
    }


    public string getDetailOrder(String _coin, string orderId)
    {
        try
        {
            var client = new RestClient("https://api.binance.com");
            String parameters = "timestamp=" + Utils.GenerateTimeStamp(DateTime.Now.ToUniversalTime()).Split(',')[0] + "&symbol=" + _coin + "&orderId=" + orderId;
            HMACSHA256 encryptor = new HMACSHA256();
            encryptor.Key = Encoding.ASCII.GetBytes(Key.getSecret());
            String sign = BitConverter.ToString(encryptor.ComputeHash(Encoding.ASCII.GetBytes(parameters))).Replace("-", "");
            parameters += "&signature=" + sign;
            var request = new RestRequest("/api/v3/order?" + parameters, Method.GET);
            request.AddHeader("X-MBX-APIKEY", Key.getKey());
            var response = client.Execute(request);
            Console.WriteLine(response.Content);
            String json = response.Content.ToString();
            JContainer dt = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));
            return json;
        }
        catch (Exception ex)
        {
            return "";
        }
        finally
        {
        }
    }

    public bool withdrawal(string address, decimal amount)
    {
        try
        {

            var client = new RestClient("https://api.binance.com");
            String parameters = "timestamp=" + Utils.GenerateTimeStamp(DateTime.Now.ToUniversalTime()).Split(',')[0] + "&asset=BTC&address=" + address + "&amount=" + amount.ToString().Replace(",", ".");
            HMACSHA256 encryptor = new HMACSHA256();
            encryptor.Key = Encoding.ASCII.GetBytes(Key.getSecret());
            String sign = BitConverter.ToString(encryptor.ComputeHash(Encoding.ASCII.GetBytes(parameters))).Replace("-", "");
            parameters += "&signature=" + sign;
            var request = new RestRequest("/wapi/v3/withdraw.html?" + parameters, Method.POST);
            request.AddHeader("X-MBX-APIKEY", Key.getKey());
            var response = client.Execute(request);
            Console.WriteLine(response.Content);
            String json = response.Content.ToString();
            JContainer dt = (JContainer)JsonConvert.DeserializeObject(json, (typeof(JContainer)));

            if (dt["success"].ToString().ToUpper() == "TRUE")
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            return false;
        }
        finally
        {
        }
    }
    

    public string getAllOrders()
    {
        try
        {
            var client = new RestClient("https://api.binance.com");
            String parameters = "timestamp=" + Utils.GenerateTimeStamp(DateTime.Now.ToUniversalTime());
            HMACSHA256 encryptor = new HMACSHA256();
            encryptor.Key = Encoding.ASCII.GetBytes(Key.getSecret());
            String sign = BitConverter.ToString(encryptor.ComputeHash(Encoding.ASCII.GetBytes(parameters))).Replace("-", "");
            parameters += "&signature=" + sign;
            var request = new RestRequest("/api/v3/openOrders?" + parameters, Method.GET);
            request.AddHeader("X-MBX-APIKEY", Key.getKey());
            var response = client.Execute(request);
            Console.WriteLine(response.Content);
            String json = response.Content.ToString();
            return json;
        }
        catch (Exception ex)
        {
            return null;
        }
        finally
        {
        }
    }

    public string getDeposits()
    {
        try
        {
            var client = new RestClient("https://api.binance.com");
            String parameters = "timestamp=" + Utils.GenerateTimeStamp(DateTime.Now.ToUniversalTime());
            HMACSHA256 encryptor = new HMACSHA256();
            encryptor.Key = Encoding.ASCII.GetBytes(Key.getSecret());
            String sign = BitConverter.ToString(encryptor.ComputeHash(Encoding.ASCII.GetBytes(parameters))).Replace("-", "");
            parameters += "&signature=" + sign;
            var request = new RestRequest("/wapi/v3/depositHistory.html?" + parameters, Method.GET);
            request.AddHeader("X-MBX-APIKEY", Key.getKey());
            var response = client.Execute(request);
            Console.WriteLine(response.Content);
            String json = response.Content.ToString();
            return json;
        }
        catch (Exception ex)
        {
            return null;
        }
        finally
        {
        }
    }

    public string getAddress(String asset)
    {
        try
        {
            var client = new RestClient("https://api.binance.com");
            String parameters = "timestamp=" + Utils.GenerateTimeStamp(DateTime.Now.ToUniversalTime()) + "&status=true&asset=" + asset;
            HMACSHA256 encryptor = new HMACSHA256();
            encryptor.Key = Encoding.ASCII.GetBytes(Key.getSecret());
            String sign = BitConverter.ToString(encryptor.ComputeHash(Encoding.ASCII.GetBytes(parameters))).Replace("-", "");
            parameters += "&signature=" + sign;
            var request = new RestRequest("/wapi/v3/depositAddress.html?" + parameters, Method.GET);
            request.AddHeader("X-MBX-APIKEY", Key.getKey());
            var response = client.Execute(request);
            Console.WriteLine(response.Content);
            String json = response.Content.ToString();
            return json;
        }
        catch (Exception ex)
        {
            return null;
        }
        finally
        {
        }
    }

    public string cancelOrder(String pair, String id)
    {
        try
        {
            return post("https://api.binance.com/api/v3/order", "symbol=" + pair.ToUpper() + "&orderId=" + id + "&timestamp=" + Utils.GenerateTimeStamp(DateTime.Now.ToUniversalTime()), Method.DELETE);
        }
        catch (Exception ex)
        {
            return null;
        }
        finally
        {
        }
    }
    
    public static decimal getVolumeBTC(string pair)
    {

        try
        {
            String jsonTicker = Http.get("https://api.binance.com/api/v1/ticker/24hr");
            JContainer jCointanerTicker = (JContainer)JsonConvert.DeserializeObject(jsonTicker, (typeof(JContainer)));

            foreach (var item in jCointanerTicker)
            {
                if (item["symbol"].ToString().Trim().ToUpper() == pair.Trim().ToUpper())
                {

                    decimal volume = decimal.Parse(item["quoteVolume"].ToString().Replace(".", ","));                    
                    return volume;
                }
            }
        }
        catch { return 0; }
        return 0;
    }

    public static decimal getPriceChangePercentCache(string pair,string cache)
    {

        try
        {
            String jsonTicker = cache;
            JContainer jCointanerTicker = (JContainer)JsonConvert.DeserializeObject(jsonTicker, (typeof(JContainer)));

            foreach (var item in jCointanerTicker)
            {
                if (item["symbol"].ToString().Trim().ToUpper() == pair.Trim().ToUpper())
                {

                    decimal priceChange = decimal.Parse(item["priceChangePercent"].ToString().Replace(".", ","));
                    return priceChange;
                }
            }
        }
        catch { return 0; }
        return 0;
    }

    public static decimal getPriceChangePercent(string pair)
    {

        try
        {
            String jsonTicker = Http.get("https://api.binance.com/api/v1/ticker/24hr");
            JContainer jCointanerTicker = (JContainer)JsonConvert.DeserializeObject(jsonTicker, (typeof(JContainer)));

            foreach (var item in jCointanerTicker)
            {
                if (item["symbol"].ToString().Trim().ToUpper() == pair.Trim().ToUpper())
                {
                    
                    decimal priceChange = decimal.Parse(item["priceChangePercent"].ToString().Replace(".", ","));
                    return priceChange;
                }
            }
        }
        catch { return 0; }
        return 0;
    }
    



    



}
