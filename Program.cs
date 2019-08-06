using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Program
{
    public static string location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";
    

    static void sleep(int i)
    {
        Logger.log("Wait " + i + "ms...");
        System.Threading.Thread.Sleep(i);
    }

    static void Main(string[] args)
    {        



        Logger.log("BOTUSD v0.0.0.1 start...");



        String jsonConfig = System.IO.File.ReadAllText(location + "key.txt");
        JContainer jConfig = (JContainer)JsonConvert.DeserializeObject(jsonConfig, (typeof(JContainer)));

        Key.keyA = jConfig["key"].ToString();
        Key.secretA = jConfig["secret"].ToString();

        decimal BUY = decimal.Parse( jConfig["buy"].ToString(),System.Globalization.NumberStyles.Float) ;
        decimal SELL = decimal.Parse(jConfig["sell"].ToString(), System.Globalization.NumberStyles.Float);
        ExchangeBinance binance = new ExchangeBinance();
        while (true)
        {

            try
            {

                String jsonOpenOrders = binance.getAllOrders();
                if (jsonOpenOrders.Length > 10)
                {
                    JContainer jContainerOpenOrders = (Newtonsoft.Json.Linq.JContainer)JsonConvert.DeserializeObject(jsonOpenOrders);
                    Logger.log( binance.cancelOrder(jContainerOpenOrders[0]["symbol"].ToString(), jContainerOpenOrders[0]["orderId"].ToString()));
                }
                Logger.log("==================================");
                Logger.log("Balances");

                string json = binance.getBalancesJSON();
                decimal TUSD = binance.getBalances("TUSD", json);
                decimal USDT = binance.getBalances("USDT", json);
                decimal USDC = binance.getBalances("USDC", json);
                decimal USDS = binance.getBalances("USDS", json);
                decimal PAX = binance.getBalances("PAX", json);

                Logger.log("TUSD: " + TUSD);
                Logger.log("USDT: " + USDT);
                Logger.log("USDC: " + USDC);
                Logger.log("USDS: " + USDS);
                Logger.log("PAX: " + PAX);
                Logger.log("==================================");

                string cache = Http.get("https://api.binance.com/api/v1/ticker/24hr");

                decimal percentBTC = ExchangeBinance.getPriceChangePercentCache("BTCUSDT", cache);

                decimal TUSDUSDT = binance.getLastPriceCACHE("TUSDUSDT", cache);
                decimal USDCUSDT = binance.getLastPriceCACHE("USDCUSDT", cache);
                decimal USDCTUSD = binance.getLastPriceCACHE("USDCTUSD", cache);
                decimal USDSUSDT = binance.getLastPriceCACHE("USDSUSDT", cache);
                decimal USDCPAX = binance.getLastPriceCACHE("USDCPAX", cache);
                decimal USDSPAX = binance.getLastPriceCACHE("USDSPAX", cache);
                decimal USDSTUSD = binance.getLastPriceCACHE("USDSTUSD", cache);
                decimal USDSUSDC = binance.getLastPriceCACHE("USDSUSDC", cache);

                string jsonOrder = string.Empty;
                if (USDT > 20)
                {
                    //BUY
                    if (TUSDUSDT <= BUY)
                    {
                        jsonOrder = binance.order("buy", "TUSDUSDT", USDT * (TUSDUSDT+0.0001m), (TUSDUSDT + 0.0001m));                        
                    }
                    //if (USDCUSDT <= BUY)
                    //{
                    //    jsonOrder = binance.order("buy", "USDCUSDT", USDT * USDCUSDT, USDCUSDT);
                    //    sleep(300000);
                    //}
                    //if (USDSUSDT <= BUY)
                    //{
                    //    jsonOrder = binance.order("buy", "USDSUSDT", USDT * USDSUSDT, USDSUSDT);
                    //    sleep(300000);
                    //}



                }

                if (TUSD > 20)
                {
                    if (TUSDUSDT >= SELL)
                    {
                        jsonOrder = binance.order("sell", "TUSDUSDT", TUSD / TUSDUSDT, TUSDUSDT );                        
                    }

                    //if (USDCTUSD <= BUY)
                    //{
                    //    jsonOrder = binance.order("buy", "USDCTUSD", TUSD * USDCTUSD, USDCTUSD);
                    //    sleep(300000);
                    //}
                    //if (USDSTUSD <= BUY)
                    //{
                    //    jsonOrder = binance.order("buy", "USDSTUSD", TUSD * USDSTUSD, USDSTUSD);
                    //    sleep(300000);
                    //}



                }


                //if (USDC > 20)
                //{
                //    if (USDCTUSD >= SELL)
                //    {
                //        jsonOrder = binance.order("sell", "TUSDUSDT", USDC / USDCTUSD, USDCTUSD);
                //        sleep(300000);
                //    }
                //    if (USDCUSDT >= SELL)
                //    {
                //        jsonOrder = binance.order("sell", "TUSDUSDT", USDC / USDCUSDT, USDCUSDT);
                //        sleep(300000);
                //    }
                //    //if (USDCPAX >= SELL)
                //    //{
                //    //    jsonOrder = binance.order("sell", "USDCPAX", USDC / USDCPAX, USDCPAX);
                //    //    sleep(300000);
                //    //}

                //    if (USDSUSDC <= BUY)
                //    {
                //        jsonOrder = binance.order("buy", "USDSUSDC", USDC * USDSUSDC, USDSUSDC);
                //        sleep(300000);
                //    }

                //    //if (USDCPAX >= SELL)
                //    //{
                //    //    jsonOrder = binance.order("sell", "USDCPAX", PAX / USDCPAX, USDCPAX);
                //    //    sleep(300000);
                //    //}

                //    if (USDCUSDT >= SELL)
                //    {
                //        jsonOrder = binance.order("sell", "USDCUSDT", USDT / USDCUSDT, USDCUSDT);
                //        sleep(300000);
                //    }
                //}

                //if (USDS > 20)
                //{
                //    if (USDSUSDT >= SELL)
                //    {
                //        jsonOrder = binance.order("sell", "USDSUSDT", USDS / USDSUSDT, USDSUSDT);
                //        sleep(300000);
                //    }
                //    //if (USDSPAX >= SELL)
                //    //{
                //    //    jsonOrder = binance.order("sell", "USDSPAX", USDS / USDSPAX, USDSPAX);
                //    //    sleep(300000);
                //    //}


                //    //if (USDSPAX > SELL)
                //    //{
                //    //    jsonOrder = binance.order("sell", "USDSPAX", PAX / USDSPAX, USDSPAX);
                //    //    sleep(300000);
                //    //}
                //}

                //if (PAX > 20)
                //{
                //    if (USDSPAX <= BUY)
                //    {
                //        jsonOrder = binance.order("buy", "USDSPAX", PAX * USDSPAX, USDSPAX);
                //        sleep(300000);
                //    }
                //    if (USDCPAX <= BUY)
                //    {
                //        jsonOrder = binance.order("buy", "USDCPAX", PAX * USDCPAX, USDCPAX);
                //        sleep(300000);
                //    }



                //}




                if (jsonOrder != string.Empty)
                {
                   

                    JContainer jContainer = (Newtonsoft.Json.Linq.JContainer)JsonConvert.DeserializeObject(jsonOrder);
                    String jsonStatusOrder = binance.getDetailOrder(jContainer["symbol"].ToString(), jContainer["orderId"].ToString());
                    for (int i = 0; i < 100; i++)
                    {
                        jsonStatusOrder = binance.getDetailOrder(jContainer["symbol"].ToString(), jContainer["orderId"].ToString());
                        Newtonsoft.Json.Linq.JContainer jContainerOrder = (Newtonsoft.Json.Linq.JContainer)JsonConvert.DeserializeObject(jsonStatusOrder);
                        if (jContainerOrder["status"].ToString().Trim().ToUpper() == "FILLED")
                            break;
                        Console.WriteLine("Wait 3s...");
                        sleep(3000);
                    }
                    
                    binance.cancelOrder(jContainer["symbol"].ToString(), jContainer["orderId"].ToString());
                }

                decimal totalUSD = (TUSD + USDT + USDC + USDS + PAX);
                Logger.log("TOTAL USD: " + totalUSD);




            }
            catch (Exception ex)
            {
                Logger.log("ERROR ::: " + ex.Message + ex.StackTrace);
            }

            Logger.log("====================================================");
            sleep(6000);
        }

    }
}

