
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 03/25/2018 20:34:38
-- Generated from EDMX file: C:\Users\g.komen\Documents\my old codes\sunami_api\sunamiapi\Models\DatabaseModel\sqldbmodel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [DB_A0A592_sunami];
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [tbl_office_id] in table 'tbl_users'
ALTER TABLE [dbo].[tbl_users]
ADD CONSTRAINT [FK_tbl_officetbl_users]
    FOREIGN KEY ([tbl_office_id])
    REFERENCES [dbo].[tbl_office]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_tbl_officetbl_users'
CREATE INDEX [IX_FK_tbl_officetbl_users]
ON [dbo].[tbl_users]
    ([tbl_office_id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------