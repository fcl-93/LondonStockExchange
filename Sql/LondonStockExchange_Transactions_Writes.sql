CREATE DATABASE [LondonStockExchange_Transactions_Writes];
GO

USE [LondonStockExchange_Transactions_Writes]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transactions](
	[TickerSymbol] [nvarchar](450) NOT NULL,
	[TradeDateTime] [datetime2](7) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[Currency] [nvarchar](max) NOT NULL,
	[ShareNumber] [decimal](18, 2) NOT NULL,
	[BrokerId] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[TickerSymbol] ASC,
	[TradeDateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
