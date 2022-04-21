%% Programado por Marcus Vinicius de Paula --- Julho de 2014
%Trabalho de Conclus�o de Curso: Identifica��o por subespa�os caixa cinza:
%teoria, m�todos e aplica��es 
%Universidade Federal de Ouro Preto
%Departamento de Engenharia El�trica
%% Descri��o
%Determina��o dos modelos de Hammerstein e Wiener de um sistema t�rmico
%tipo SISO e de um sistema de n�vel tipo MIMO

%% Menu inicial
clc; clear; close all;
disp('Op��es');
disp('1: Sistemas t�rmico');
disp('2: Sistema de n�vel');
opcao = input('Digite 1 ou 2:');
while opcao ~= 1 && opcao ~= 2
    opcao = input('Digite 1 ou 2:');
end
%% Sistema T�rmico
if opcao == 1
    disp('Op��es');
    disp('3: Modelo de Wiener');
    disp('4: Modelo de Hammerstein');
    opcao = input('Digite 3 ou 4:');
    while opcao ~= 3 && opcao ~= 4
        opcao = input('Digite 3 ou 4:');
    end  
    if opcao == 3 %Modelo de Wiener 
        load('entrada_estatica_win_ter');
        load('saida_estatica_win_ter');
        load('entrada_ter');
        load('saida_ter');
        load('tempo_ter');
        load('tempo_ter_est')
        curva_estat_aux = curvas_estaticas_win(entrada_estatica_win_ter, saida_estatica_win_ter,tempo_ter_est); 
        [A, B, C, D, curvas_estaticas_ident, indice_RMSE] = identificacao_dinamica_win(curva_estat_aux, entrada_ter, saida_ter, tempo_ter);
    end
    if opcao == 4 %Modelo de Hammerstein 
        load('entrada_estatica_ham_ter');
        load('saida_estatica_ham_ter');
        load('entrada_ter');
        load('saida_ter');
        load('tempo_ter');
        load('tempo_ter_est');
        curva_estat_aux = curvas_estaticas_ham(entrada_estatica_ham_ter, saida_estatica_ham_ter, tempo_ter_est); 
        [A, B, C, D, curvas_estaticas_ident, indice_RMSE] = identificacao_dinamica_ham(curva_estat_aux, entrada_ter, saida_ter, tempo_ter); 
    end      
end
%% Sistema de N�vel
if opcao == 2
    disp('Op��es');
    disp('3: Modelo de Wiener');
    disp('4: Modelo de Hammerstein');
    opcao = input('Digite 3 ou 4:');
    while opcao ~= 3 && opcao ~= 4
        opcao = input('Digite 3 ou 4:');
    end 
    if opcao == 3 %Modelo de Wiener 
        load('entrada_estatica_win_niv');
        load('saida_estatica_win_niv');
        load('entrada_niv');
        load('saida_niv');
        load('tempo_niv');
        load('tempo_niv_est');
        curva_estat_aux = curvas_estaticas_win(entrada_estatica_win_niv, saida_estatica_win_niv, tempo_niv_est);
        [A, B, C, D, curvas_estaticas_ident, indice_RMSE] = identificacao_dinamica_win(curva_estat_aux, entrada_niv, saida_niv, tempo_niv);
    end
    if opcao == 4 %Modelo de Hammerstein 
        load('entrada_estatica_ham_niv');
        load('saida_estatica_ham_niv');
        load('entrada_niv');
        load('saida_niv');
        load('tempo_niv');
        load('tempo_niv_est');
        curva_estat_aux = curvas_estaticas_ham(entrada_estatica_ham_niv, saida_estatica_ham_niv, tempo_niv_est);
        [A, B, C, D, curvas_estaticas_ident, indice_RMSE] = identificacao_dinamica_ham(curva_estat_aux, entrada_niv, saida_niv, tempo_niv);
    end   
end 
