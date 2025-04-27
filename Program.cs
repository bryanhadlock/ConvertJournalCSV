// Copyright (c) AvantGuard Monitoring Centers. All rights reserved.

using ConvertJournalCSV;

args ??= [];

using var reader = new StreamReader(@"C:\Users\bhadlock\Downloads\Trade_Export.csv");

var trades = new List<Trade>();

var count = 0;
while (!reader.EndOfStream)
{
    var line = reader.ReadLine();
    if (count > 0)
    {
        var values = line.Split(',');

        var time = values[0].Trim('"').Split(";");

        var trade = new Trade
        {
            Time = TimeOnly.Parse(time[1]),
            TimeString = time[1],
            Symbol = values[1].Trim('"'),
            BuySell = values[2].Trim('"'),
            Price = values[3].Trim('"'),
            Quantity = values[4].Trim('"'),
            Account = values[5].Trim('"'),
        };
        trades.Add(trade);
    }
    count++;
}

var sortedTrades = trades.OrderBy(x => x.Time).ToList();
var tradeStack = new List<TradeSide>();

using var sw = new StreamWriter(@"C:\Users\bhadlock\Downloads\trade_converted.csv");
sw.WriteLine("Time,Symbol,Side,Price,Qty,Route,Broker,Account,Type,Cloid");
foreach (var trade in sortedTrades)
{

    var tradeSide = tradeStack.SingleOrDefault(x=> x.Symbol == trade.Symbol);
    var quantity = GetQuanity(trade);
    if (tradeSide == null)
    {
        
        var tradeSidedd = new TradeSide
        {
            Quanity = quantity,
            Symbol = trade.Symbol,
        };
        if (trade.BuySell == "SELL")
        {
            tradeSidedd.Side = Side.Short;
            sw.WriteLine($"{trade.TimeString},{trade.Symbol},SS,{trade.Price},{quantity},SMRT,BEST,U8490678,Short,1");
        }
        else
        {
            tradeSidedd.Side = Side.Long;
            sw.WriteLine($"{trade.TimeString},{trade.Symbol},B,{trade.Price},{quantity},SMRT,BEST,U8490678,Margin,1");
        }
        tradeStack.Add(tradeSidedd);
    }
    else
    {
        if (tradeSide.Side == Side.Short && trade.BuySell == "SELL")
        {
            tradeSide.Quanity = tradeSide.Quanity + quantity;
            sw.WriteLine($"{trade.TimeString},{trade.Symbol},SS,{trade.Price},{quantity},SMRT,BEST,U8490678,Short,1");
        }
        else if (tradeSide.Side == Side.Long && trade.BuySell == "BUY")
        {
            tradeSide.Quanity = tradeSide.Quanity + quantity;
            sw.WriteLine($"{trade.TimeString},{trade.Symbol},B,{trade.Price},{quantity},SMRT,BEST,U8490678,Margin,1");
        }
        else
        {
            if (trade.BuySell == "SELL")
            {
                sw.WriteLine($"{trade.TimeString},{trade.Symbol},S,{trade.Price},{quantity},SMRT,BEST,U8490678,Margin,1");
            }

            if (trade.BuySell == "BUY") 
            {
                sw.WriteLine($"{trade.TimeString},{trade.Symbol},B,{trade.Price},{quantity},SMRT,BEST,U8490678,Margin,1");
            }
            tradeSide.Quanity = tradeSide.Quanity - quantity;

            if (tradeSide.Quanity == 0)
            {
                tradeStack.Remove(tradeSide);
            }
        }
    }
}

static int GetQuanity(Trade? trade)
{
    return int.Parse(trade.Quantity) > 0 ? int.Parse(trade.Quantity) : int.Parse(trade.Quantity) * -1;
}