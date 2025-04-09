CREATE DATABASE readit;

CREATE TABLE Imagens (
    img_id INT IDENTITY(1,1) PRIMARY KEY,
    img_imagem VARBINARY(MAX) NOT NULL,
    img_formato VARCHAR(10) NOT NULL,
    img_dataInclusao DATETIME DEFAULT GETDATE(),
    img_dataAtualizacao DATETIME NULL,
    img_tipo TINYINT NOT NULL
);

CREATE TABLE Usuarios (
    usu_id INT IDENTITY(1,1) PRIMARY KEY,
    usu_nome VARCHAR(100) NOT NULL,
    usu_apelido VARCHAR(50) NOT NULL,
    usu_email VARCHAR(100) NOT NULL,
    usu_senha VARCHAR(255) NOT NULL,
	usu_administrador BIT NOT NULL,
	img_id INT NULL,
	CONSTRAINT FK_UsuarioImagens FOREIGN KEY (img_id) REFERENCES Imagens(img_id)
);

CREATE TABLE Preferencias (
    pre_id INT IDENTITY(1,1) PRIMARY KEY,
    pre_preferencia VARCHAR (100) NOT NULL
);

CREATE TABLE PreferenciasUsuario (
    pfu_id INT IDENTITY(1,1) PRIMARY KEY,
	usu_id INT NOT NULL,
	pre_id INT NOT NULL,
	CONSTRAINT FK_PreferenciasUsuarioUsuarios FOREIGN KEY (usu_id) REFERENCES Usuarios(usu_id),
	CONSTRAINT FK_PreferenciasUsuarioPreferencias FOREIGN KEY (pre_id) REFERENCES Preferencias(pre_id)
);

CREATE TABLE Obras (
    obs_id INT IDENTITY(1,1) PRIMARY KEY,
    obs_nomeObra VARCHAR(255) NOT NULL,
    obs_status TINYINT NOT NULL,
    obs_tipo TINYINT NOT NULL,
    obs_descricao VARCHAR(2000) NULL,
    obs_dataPublicacao DATETIME DEFAULT GETDATE(),
    obs_dataAtualizacao DATETIME NULL,
	usu_id INT NOT NULL,
	img_id INT NOT NULL,
	CONSTRAINT FK_ObrasUsuarios FOREIGN KEY (usu_id) REFERENCES Usuarios(usu_id),
	CONSTRAINT FK_ObrasImagens FOREIGN KEY (img_id) REFERENCES Imagens(img_id)
);

CREATE TABLE BookmarksUsuario (
    bku_id INT IDENTITY(1,1) PRIMARY KEY,
	usu_id INT NOT NULL,
	obs_id INT NOT NULL,
	CONSTRAINT FK_BookmarksUsuarioUsuarios FOREIGN KEY (usu_id) REFERENCES Usuarios(usu_id),
	CONSTRAINT FK_BookmarksUsuarioObras FOREIGN KEY (obs_id) REFERENCES Obras(obs_id)
);

CREATE TABLE Generos (
    gns_id INT IDENTITY(1,1) PRIMARY KEY,
	gns_nome VARCHAR(255) NOT NULL,
);

CREATE TABLE ObrasGeneros (
    ogs_id INT IDENTITY(1,1) PRIMARY KEY,
	obs_id INT NOT NULL,
	gns_id INT NOT NULL,
	CONSTRAINT FK_ObrasGenerosObras FOREIGN KEY (obs_id) REFERENCES Obras(obs_id),
	CONSTRAINT FK_ObrasGenerosGeneros FOREIGN KEY (gns_id) REFERENCES Generos(gns_id)
);

CREATE TABLE AvaliacoesObra (
    avo_id INT IDENTITY(1,1) PRIMARY KEY,
    avo_nota TINYINT NOT NULL,
    avo_dataAvaliacao DATETIME DEFAULT GETDATE(),
    avo_dataAtualizacao DATETIME NULL,
	usu_id INT NOT NULL,
	obs_id INT NOT NULL,
	CONSTRAINT FK_AvaliacoesObraUsuarios FOREIGN KEY (usu_id) REFERENCES Usuarios(usu_id),
	CONSTRAINT FK_AvaliacoesObraObras FOREIGN KEY (obs_id) REFERENCES Obras(obs_id)
);

CREATE TABLE CapitulosObra (
    cpo_id INT IDENTITY(1,1) PRIMARY KEY,
    cpo_numeroCapitulo INT NOT NULL,
    cpo_dataPublicacao DATETIME DEFAULT GETDATE(),
    cpo_dataAtualizacao DATETIME NULL,
	usu_id INT NOT NULL,
	obs_id INT NOT NULL,
	CONSTRAINT FK_CapitulosObraUsuarios FOREIGN KEY (usu_id) REFERENCES Usuarios(usu_id),
	CONSTRAINT FK_CapitulosObraObras FOREIGN KEY (obs_id) REFERENCES Obras(obs_id)
);

CREATE TABLE PaginasCapitulo (
    pgc_id INT IDENTITY(1,1) PRIMARY KEY,
    pgc_numeroPagina INT NOT NULL,
    pgc_pagina VARBINARY(MAX) NOT NULL,
    pgc_tamanho VARCHAR(50) NOT NULL,
	cpo_id INT NOT NULL,
	CONSTRAINT FK_PaginasCapituloCapitulosObra FOREIGN KEY (cpo_id) REFERENCES CapitulosObra(cpo_id)
);

CREATE TABLE VisualizacoesObra (
    vso_id INT IDENTITY(1,1) PRIMARY KEY,
    vso_views INT NOT NULL,
    obs_id INT NOT NULL,
	CONSTRAINT FK_VisualizacoesObraObras FOREIGN KEY (obs_id) REFERENCES Obras(obs_id)
);

CREATE TABLE Comentarios (
    cts_id INT IDENTITY(1,1) PRIMARY KEY,
    cts_comentario VARCHAR(255) NOT NULL,
    cts_data DATETIME DEFAULT GETDATE(),
    cts_dataAtualizacao DATETIME NULL,
    obs_id INT NOT NULL,
    usu_id INT NOT NULL,
    cpo_id INT NULL,
	CONSTRAINT FK_ComentariosObras FOREIGN KEY (obs_id) REFERENCES Obras(obs_id),
	CONSTRAINT FK_ComentariosUsuarios FOREIGN KEY (usu_id) REFERENCES Usuarios(usu_id),
	CONSTRAINT FK_ComentariosCapitulosObra FOREIGN KEY (cpo_id) REFERENCES CapitulosObra(cpo_id)
);

CREATE TABLE RespostasComentario (
    rsc_id INT IDENTITY(1,1) PRIMARY KEY,
    cts_id INT NOT NULL,
    res_id INT NOT NULL,
	CONSTRAINT FK_RespostasComentarioComentarios FOREIGN KEY (cts_id) REFERENCES Comentarios(cts_id),
	CONSTRAINT FK_RespostasComentarioResposta FOREIGN KEY (res_id) REFERENCES Comentarios(cts_id)
);

CREATE TABLE Avaliacoes (
    ava_id INT IDENTITY(1,1) PRIMARY KEY,
    ava_avaliacao VARCHAR(20) NOT NULL,
);

CREATE TABLE AvaliacoesComentario (
    avc_id INT IDENTITY(1,1) PRIMARY KEY,
	cts_id INT NOT NULL,
	ava_id INT NOT NULL,
	usu_id INT NOT NULL,
	CONSTRAINT FK_AvaliacoesComentarioComentarios FOREIGN KEY (cts_id) REFERENCES Comentarios(cts_id),
	CONSTRAINT FK_AvaliacoesComentarioAvaliacoes FOREIGN KEY (ava_id) REFERENCES Avaliacoes(ava_id),
    CONSTRAINT FK_AvaliacoesComentarioUsuarios FOREIGN KEY (usu_id) REFERENCES [dbo].[Usuarios] (usu_id)
);