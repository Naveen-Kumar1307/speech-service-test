
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 06/30/2011 19:54:59
-- Generated from EDMX file: C:\GlobalEnglish2\Release_3\Development\WebServices\Services\Recognition\Repository\RepositoryModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [SpeechRecognition];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_RecognitionAttemptRecognitionRequest]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RecognitionAttempts] DROP CONSTRAINT [FK_RecognitionAttemptRecognitionRequest];
GO
IF OBJECT_ID(N'[dbo].[FK_RecognitionAttemptRecognitionResult]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RecognitionAttempts] DROP CONSTRAINT [FK_RecognitionAttemptRecognitionResult];
GO
IF OBJECT_ID(N'[dbo].[FK_RecognitionRequestExpectedResult]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ExpectedResults] DROP CONSTRAINT [FK_RecognitionRequestExpectedResult];
GO
IF OBJECT_ID(N'[dbo].[FK_RecognitionResultAudioQuality]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RecognitionResults] DROP CONSTRAINT [FK_RecognitionResultAudioQuality];
GO
IF OBJECT_ID(N'[dbo].[FK_RecognitionResultSentenceMatch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RecognitionResults] DROP CONSTRAINT [FK_RecognitionResultSentenceMatch];
GO
IF OBJECT_ID(N'[dbo].[FK_SentenceMatchSentenceQuality]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SentenceMatches] DROP CONSTRAINT [FK_SentenceMatchSentenceQuality];
GO
IF OBJECT_ID(N'[dbo].[FK_SentenceQualityPhonemeQuality]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PhonemeQualities] DROP CONSTRAINT [FK_SentenceQualityPhonemeQuality];
GO
IF OBJECT_ID(N'[dbo].[FK_SentenceQualityWordQuality]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WordQualities] DROP CONSTRAINT [FK_SentenceQualityWordQuality];
GO
IF OBJECT_ID(N'[dbo].[FK_UserPhonemeQuality]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PhonemeQualities] DROP CONSTRAINT [FK_UserPhonemeQuality];
GO
IF OBJECT_ID(N'[dbo].[FK_UserRecognitionAttempt]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RecognitionAttempts] DROP CONSTRAINT [FK_UserRecognitionAttempt];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AudioQualities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AudioQualities];
GO
IF OBJECT_ID(N'[dbo].[ExpectedResults]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ExpectedResults];
GO
IF OBJECT_ID(N'[dbo].[PhonemeQualities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PhonemeQualities];
GO
IF OBJECT_ID(N'[dbo].[RecognitionAttempts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RecognitionAttempts];
GO
IF OBJECT_ID(N'[dbo].[RecognitionRequests]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RecognitionRequests];
GO
IF OBJECT_ID(N'[dbo].[RecognitionResults]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RecognitionResults];
GO
IF OBJECT_ID(N'[dbo].[SentenceMatches]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SentenceMatches];
GO
IF OBJECT_ID(N'[dbo].[SentenceQualities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SentenceQualities];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[WordQualities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WordQualities];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'RecognitionRequests'
CREATE TABLE [dbo].[RecognitionRequests] (
    [RecognitionType] int  NOT NULL,
    [Grammar] nvarchar(max)  NULL,
    [Id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'ExpectedResults'
CREATE TABLE [dbo].[ExpectedResults] (
    [Answer] nvarchar(max)  NULL,
    [FullAnswer] nvarchar(max)  NULL,
    [Id] int IDENTITY(1,1) NOT NULL,
    [RecognitionRequest_Id] int  NOT NULL
);
GO

-- Creating table 'RecognitionResults'
CREATE TABLE [dbo].[RecognitionResults] (
    [Type] int  NOT NULL,
    [ResultDetail] int  NOT NULL,
    [Message] nvarchar(max)  NULL,
    [RecognitionTime] int  NOT NULL,
    [QueuedRecognitionTime] int  NOT NULL,
    [RecordedFileName] nvarchar(max)  NULL,
    [RecordedFileType] nvarchar(max)  NULL,
    [Id] int IDENTITY(1,1) NOT NULL,
    [Sentence_Id] int  NOT NULL,
    [AudioMeasure_Id] int  NOT NULL
);
GO

-- Creating table 'AudioQualities'
CREATE TABLE [dbo].[AudioQualities] (
    [Level] int  NOT NULL,
    [Truncation] int  NOT NULL,
    [SignalNoiseRatio] real  NOT NULL,
    [NoiseLevel] int  NOT NULL,
    [Miscellany] nvarchar(max)  NULL,
    [Id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'SentenceMatches'
CREATE TABLE [dbo].[SentenceMatches] (
    [RecognizedText] nvarchar(max)  NULL,
    [Interpretation] nvarchar(max)  NULL,
    [MatchedIndex] int  NOT NULL,
    [Id] int IDENTITY(1,1) NOT NULL,
    [Quality_Id] int  NOT NULL
);
GO

-- Creating table 'SentenceQualities'
CREATE TABLE [dbo].[SentenceQualities] (
    [FrameCount] int  NOT NULL,
    [Confidence] int  NOT NULL,
    [Score] int  NOT NULL,
    [PhraseConfidence] int  NOT NULL,
    [PhraseAcceptanceThreshold] int  NOT NULL,
    [SentenceAcceptanceThreshold] int  NOT NULL,
    [Id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'WordQualities'
CREATE TABLE [dbo].[WordQualities] (
    [PhoneCount] int  NOT NULL,
    [StartFrame] int  NOT NULL,
    [EndFrame] int  NOT NULL,
    [FrameCount] int  NOT NULL,
    [Confidence] int  NOT NULL,
    [Score] int  NOT NULL,
    [Accepted] bit  NOT NULL,
    [Id] int IDENTITY(1,1) NOT NULL,
    [SentenceQuality_Id] int  NOT NULL
);
GO

-- Creating table 'RecognitionAttempts'
CREATE TABLE [dbo].[RecognitionAttempts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EngineId] int  NOT NULL,
    [UserId] int  NOT NULL,
    [RecognitionRequest_Id] int  NOT NULL,
    [RecognitionResult_Id] int  NOT NULL
);
GO

-- Creating table 'PhonemeQualities'
CREATE TABLE [dbo].[PhonemeQualities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PhoneName] nvarchar(3)  NOT NULL,
    [Grapheme] nvarchar(4)  NOT NULL,
    [Score] int  NOT NULL,
    [UserId] int  NOT NULL,
    [SentenceQualityId] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ClsUserId] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'RecognitionRequests'
ALTER TABLE [dbo].[RecognitionRequests]
ADD CONSTRAINT [PK_RecognitionRequests]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ExpectedResults'
ALTER TABLE [dbo].[ExpectedResults]
ADD CONSTRAINT [PK_ExpectedResults]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RecognitionResults'
ALTER TABLE [dbo].[RecognitionResults]
ADD CONSTRAINT [PK_RecognitionResults]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AudioQualities'
ALTER TABLE [dbo].[AudioQualities]
ADD CONSTRAINT [PK_AudioQualities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SentenceMatches'
ALTER TABLE [dbo].[SentenceMatches]
ADD CONSTRAINT [PK_SentenceMatches]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SentenceQualities'
ALTER TABLE [dbo].[SentenceQualities]
ADD CONSTRAINT [PK_SentenceQualities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WordQualities'
ALTER TABLE [dbo].[WordQualities]
ADD CONSTRAINT [PK_WordQualities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RecognitionAttempts'
ALTER TABLE [dbo].[RecognitionAttempts]
ADD CONSTRAINT [PK_RecognitionAttempts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PhonemeQualities'
ALTER TABLE [dbo].[PhonemeQualities]
ADD CONSTRAINT [PK_PhonemeQualities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [RecognitionRequest_Id] in table 'ExpectedResults'
ALTER TABLE [dbo].[ExpectedResults]
ADD CONSTRAINT [FK_RecognitionRequestExpectedResult]
    FOREIGN KEY ([RecognitionRequest_Id])
    REFERENCES [dbo].[RecognitionRequests]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RecognitionRequestExpectedResult'
CREATE INDEX [IX_FK_RecognitionRequestExpectedResult]
ON [dbo].[ExpectedResults]
    ([RecognitionRequest_Id]);
GO

-- Creating foreign key on [SentenceQuality_Id] in table 'WordQualities'
ALTER TABLE [dbo].[WordQualities]
ADD CONSTRAINT [FK_SentenceQualityWordQuality]
    FOREIGN KEY ([SentenceQuality_Id])
    REFERENCES [dbo].[SentenceQualities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SentenceQualityWordQuality'
CREATE INDEX [IX_FK_SentenceQualityWordQuality]
ON [dbo].[WordQualities]
    ([SentenceQuality_Id]);
GO

-- Creating foreign key on [Quality_Id] in table 'SentenceMatches'
ALTER TABLE [dbo].[SentenceMatches]
ADD CONSTRAINT [FK_SentenceMatchSentenceQuality]
    FOREIGN KEY ([Quality_Id])
    REFERENCES [dbo].[SentenceQualities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SentenceMatchSentenceQuality'
CREATE INDEX [IX_FK_SentenceMatchSentenceQuality]
ON [dbo].[SentenceMatches]
    ([Quality_Id]);
GO

-- Creating foreign key on [Sentence_Id] in table 'RecognitionResults'
ALTER TABLE [dbo].[RecognitionResults]
ADD CONSTRAINT [FK_RecognitionResultSentenceMatch]
    FOREIGN KEY ([Sentence_Id])
    REFERENCES [dbo].[SentenceMatches]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RecognitionResultSentenceMatch'
CREATE INDEX [IX_FK_RecognitionResultSentenceMatch]
ON [dbo].[RecognitionResults]
    ([Sentence_Id]);
GO

-- Creating foreign key on [AudioMeasure_Id] in table 'RecognitionResults'
ALTER TABLE [dbo].[RecognitionResults]
ADD CONSTRAINT [FK_RecognitionResultAudioQuality]
    FOREIGN KEY ([AudioMeasure_Id])
    REFERENCES [dbo].[AudioQualities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RecognitionResultAudioQuality'
CREATE INDEX [IX_FK_RecognitionResultAudioQuality]
ON [dbo].[RecognitionResults]
    ([AudioMeasure_Id]);
GO

-- Creating foreign key on [RecognitionRequest_Id] in table 'RecognitionAttempts'
ALTER TABLE [dbo].[RecognitionAttempts]
ADD CONSTRAINT [FK_RecognitionAttemptRecognitionRequest]
    FOREIGN KEY ([RecognitionRequest_Id])
    REFERENCES [dbo].[RecognitionRequests]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RecognitionAttemptRecognitionRequest'
CREATE INDEX [IX_FK_RecognitionAttemptRecognitionRequest]
ON [dbo].[RecognitionAttempts]
    ([RecognitionRequest_Id]);
GO

-- Creating foreign key on [RecognitionResult_Id] in table 'RecognitionAttempts'
ALTER TABLE [dbo].[RecognitionAttempts]
ADD CONSTRAINT [FK_RecognitionAttemptRecognitionResult]
    FOREIGN KEY ([RecognitionResult_Id])
    REFERENCES [dbo].[RecognitionResults]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RecognitionAttemptRecognitionResult'
CREATE INDEX [IX_FK_RecognitionAttemptRecognitionResult]
ON [dbo].[RecognitionAttempts]
    ([RecognitionResult_Id]);
GO

-- Creating foreign key on [UserId] in table 'RecognitionAttempts'
ALTER TABLE [dbo].[RecognitionAttempts]
ADD CONSTRAINT [FK_UserRecognitionAttempt]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserRecognitionAttempt'
CREATE INDEX [IX_FK_UserRecognitionAttempt]
ON [dbo].[RecognitionAttempts]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'PhonemeQualities'
ALTER TABLE [dbo].[PhonemeQualities]
ADD CONSTRAINT [FK_UserPhonemeQuality]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserPhonemeQuality'
CREATE INDEX [IX_FK_UserPhonemeQuality]
ON [dbo].[PhonemeQualities]
    ([UserId]);
GO

-- Creating foreign key on [SentenceQualityId] in table 'PhonemeQualities'
ALTER TABLE [dbo].[PhonemeQualities]
ADD CONSTRAINT [FK_SentenceQualityPhonemeQuality]
    FOREIGN KEY ([SentenceQualityId])
    REFERENCES [dbo].[SentenceQualities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SentenceQualityPhonemeQuality'
CREATE INDEX [IX_FK_SentenceQualityPhonemeQuality]
ON [dbo].[PhonemeQualities]
    ([SentenceQualityId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------