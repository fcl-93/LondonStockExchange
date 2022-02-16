﻿using Dapper;
using Microsoft.Data.SqlClient;

namespace LondonStockExchange.DataProcessing.Read.Api.Infrastructure.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly string connectionString;

        public StockRepository(string connectionString)
        {
            this.connectionString=connectionString;
        }

        public async Task<dynamic> GetValueByTickerSymbol(string tickerSymbol)
        {
            var sql = @"SELECT TOP 1 [TickerSymbol], [TradeDateTime], [Price],[Currency] 
                        FROM[LondonStockExchange_Transactions_Writes].[dbo].[Transactions]
                        WHERE TickerSymbol = @TickerSymbol
                        ORDER BY TradeDateTime DESC";

            await using var connection = new SqlConnection();
            await connection.OpenAsync();

            var lastStockPrice = await connection.QueryFirstOrDefaultAsync<object>(sql, new { TickerSymbol = tickerSymbol });
            return lastStockPrice;
        }

        public async Task<IEnumerable<dynamic>> GetValuesByTickerSymbols(IEnumerable<string> tickerSymbols)
        {
            var sql = @"
                Select t1.TickerSymbol, t1.Price, t1.Currency, t1.TradeDateTime
                from [LondonStockExchange_Transactions_Writes].[dbo].[Transactions] as t1
                inner join 
                (
                    SELECT t2.TickerSymbol, MAX(t2.[TradeDateTime]) as LastestDate
		            FROM [LondonStockExchange_Transactions_Writes].[dbo].[Transactions] as t2
		            WHERE t2.TickerSymbol In @TickerSymbols
		            Group by t2.TickerSymbol 
                ) t 
                on t1.TradeDateTime = t.LastestDate
                and t1.TickerSymbol = t.TickerSymbol
                ";

            await using var connection = new SqlConnection("Server=localhost;Database=LondonStockExchange_Transactions_Writes;Integrated Security=SSPI;MultipleActiveResultSets=true;TrustServerCertificate=True");
            await connection.OpenAsync();

            var lastStockPricesForTickers = await connection.QueryAsync<object>(sql, new { TickerSymbols = tickerSymbols.ToArray() });
            return lastStockPricesForTickers;
        }

        public async Task<IEnumerable<dynamic>> GetValuesForAllTickers(int pageNumber, int pageSize)
        {
            var sql = @"Select t1.TickerSymbol, t1.Price, t1.Currency, t1.TradeDateTime
                        from [LondonStockExchange_Transactions_Writes].[dbo].[Transactions] as t1
                        inner join 
                        (
                            SELECT t2.TickerSymbol, MAX(t2.[TradeDateTime]) as LastestDate
		                    FROM [LondonStockExchange_Transactions_Writes].[dbo].[Transactions] as t2
		                    Group by t2.TickerSymbol 
					        Order By t2.TickerSymbol
					        OFFSET (@PageNumber-1)*@PageSize ROWS
					        FETCH NEXT @PageSize ROWS ONLY
                        ) t 
                        on t1.TradeDateTime = t.LastestDate
                        and t1.TickerSymbol = t.TickerSymbol";

            await using var connection = new SqlConnection("Server=localhost;Database=LondonStockExchange_Transactions_Writes;Integrated Security=SSPI;MultipleActiveResultSets=true;TrustServerCertificate=True");
            await connection.OpenAsync();

            var lastStockPricesForTickers = await connection.QueryAsync<object>(sql, new { PageNumber = pageNumber, PageSize = pageSize });
            return lastStockPricesForTickers;
        }
    }
}
