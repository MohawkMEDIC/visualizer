ALTER TABLE [dbo].[AuditStatus] DROP CONSTRAINT [FK_StatusCodeId_StatusCode]
GO

ALTER TABLE [dbo].[AuditStatus] DROP CONSTRAINT [FK_AuditId_Audit]
GO

ALTER TABLE [dbo].[AuditSourceTypeAssoc] DROP CONSTRAINT [FK_AuditSourceTypeAssoc_Code]
GO

ALTER TABLE [dbo].[AuditSourceTypeAssoc] DROP CONSTRAINT [FK_AuditSourceTypeAssoc_AuditSource]
GO

ALTER TABLE [dbo].[AuditParticipantRoleCodeAssoc] DROP CONSTRAINT [FK_AuditParticipantRoleCodeAssoc_AuditParticipant]
GO

ALTER TABLE [dbo].[AuditParticipantRoleCodeAssoc] DROP CONSTRAINT [FK_AuditParticipantRoleCodeAssoc_AuditCode]
GO

ALTER TABLE [dbo].[AuditParticipant] DROP CONSTRAINT [FKAuditParticipant_NodeVersion]
GO

ALTER TABLE [dbo].[AuditParticipant] DROP CONSTRAINT [FK_AuditParticipant_Audit]
GO

ALTER TABLE [dbo].[AuditParticipant] DROP CONSTRAINT [FK_AuditParticipant_AspNetUser]
GO

ALTER TABLE [dbo].[AuditObjectDetail] DROP CONSTRAINT [FK_AuditObjectDetail_AuditableObject]
GO

ALTER TABLE [dbo].[AuditObject] DROP CONSTRAINT [FK_AuditObject_TypeCode_Code]
GO

ALTER TABLE [dbo].[AuditObject] DROP CONSTRAINT [FK_AuditObject_RoleCodeId_Code]
GO

ALTER TABLE [dbo].[AuditObject] DROP CONSTRAINT [FK_AuditObject_LifecycleCode_Code]
GO

ALTER TABLE [dbo].[AuditObject] DROP CONSTRAINT [FK_AuditObject_IdTypeCodeId_Code]
GO

ALTER TABLE [dbo].[AuditObject] DROP CONSTRAINT [FK_AuditObject_Audit]
GO

ALTER TABLE [dbo].[AuditEventTypeAuditCodeAssoc] DROP CONSTRAINT [FK_AuditEventTypeAuditCodeAssoc_Code]
GO

ALTER TABLE [dbo].[AuditEventTypeAuditCodeAssoc] DROP CONSTRAINT [FK_AuditEventTypeAuditCodeAssoc_Audit]
GO

ALTER TABLE [dbo].[AuditAuditSourceAssoc] DROP CONSTRAINT [FK_AuditAuditSourceAssoc_AuditSource]
GO

ALTER TABLE [dbo].[AuditAuditSourceAssoc] DROP CONSTRAINT [FK_AuditAuditSourceAssoc_Audit]
GO

ALTER TABLE [dbo].[Audit] DROP CONSTRAINT [FK_OutcomeCodeId_Code]
GO

ALTER TABLE [dbo].[Audit] DROP CONSTRAINT [FK_EventCodeId_Code]
GO

ALTER TABLE [dbo].[Audit] DROP CONSTRAINT [FK_ActionCodeId_Code]
GO


/****** Object:  Table [dbo].[AuditStatus]    Script Date: 4/9/2014 11:49:53 AM ******/
DROP TABLE [dbo].[AuditStatus]
GO

/****** Object:  Table [dbo].[AuditSourceTypeAssoc]    Script Date: 4/9/2014 11:49:53 AM ******/
DROP TABLE [dbo].[AuditSourceTypeAssoc]
GO

/****** Object:  Table [dbo].[AuditSource]    Script Date: 4/9/2014 11:49:53 AM ******/
DROP TABLE [dbo].[AuditSource]
GO

/****** Object:  Table [dbo].[AuditParticipantRoleCodeAssoc]    Script Date: 4/9/2014 11:49:53 AM ******/
DROP TABLE [dbo].[AuditParticipantRoleCodeAssoc]
GO

/****** Object:  Table [dbo].[AuditParticipant]    Script Date: 4/9/2014 11:49:53 AM ******/
DROP TABLE [dbo].[AuditParticipant]
GO

/****** Object:  Table [dbo].[AuditObjectDetail]    Script Date: 4/9/2014 11:49:53 AM ******/
DROP TABLE [dbo].[AuditObjectDetail]
GO

/****** Object:  Table [dbo].[AuditObject]    Script Date: 4/9/2014 11:49:53 AM ******/
DROP TABLE [dbo].[AuditObject]
GO

/****** Object:  Table [dbo].[AuditEventTypeAuditCodeAssoc]    Script Date: 4/9/2014 11:49:53 AM ******/
DROP TABLE [dbo].[AuditEventTypeAuditCodeAssoc]
GO

/****** Object:  Table [dbo].[AuditCode]    Script Date: 4/9/2014 11:49:53 AM ******/
DROP TABLE [dbo].[AuditCode]
GO

/****** Object:  Table [dbo].[AuditAuditSourceAssoc]    Script Date: 4/9/2014 11:49:53 AM ******/
DROP TABLE [dbo].[AuditAuditSourceAssoc]
GO

/****** Object:  Table [dbo].[Audit]    Script Date: 4/9/2014 11:49:53 AM ******/
DROP TABLE [dbo].[Audit]
GO


/****** Object:  StoredProcedure [dbo].[sp_CreateAuditCodeIfNotExists]    Script Date: 4/9/2014 11:50:10 AM ******/
DROP PROCEDURE [dbo].[sp_CreateAuditCodeIfNotExists]
GO

/****** Object:  UserDefinedFunction [dbo].[fn_GetAuditCodeId]    Script Date: 4/9/2014 11:50:32 AM ******/
DROP FUNCTION [dbo].[fn_GetAuditCodeId]
GO
DROP TABLE NodeVersion;
DROP TABLE StatusCode;
DROP TABLE Node;
GO