%% Identificação da parte linear do modelo de Wiener, determinação da(s)
%% melhor(es) curva(s) estática(s) e validação
function [A, B, C, D, curvas_estaticas_ident, indice_RMSE] = identificacao_dinamica_win(curva_estat_aux, entrada, saida, tempo)
    dimensao_entrada = size(entrada, 2);
    dimensao_saida = size(saida,2);
    num_amostras_descartadas = round(0.20*size(entrada,1)); %Quantidade de amostras descartadas (regime transitório)
    num_amostras_identific = round(size(entrada,1) - num_amostras_descartadas)/2; %Quantidade de amostras de identificação
    for k=1:1:size(curva_estat_aux,1)
        for j=1:1:dimensao_saida
            curva_estat_sym{k,j} = poly2sym(curva_estat_aux{k,j},'z'); %Curvas estáticas
            inv_curva_estat{k,j} = finverse(curva_estat_sym{k,j}); %Inversas das curvas estáticas  
        end
    end
    clc;
    entrada_identificacao = entrada(num_amostras_descartadas+1:num_amostras_descartadas+num_amostras_identific,:); %Amostras do(s) sinal(is) de entrada para identificação
    entrada_validacao = entrada(num_amostras_descartadas+num_amostras_identific+1:size(entrada,1),:); %Amostras do(s) sinal(is) de entrada para validação
    saida_validacao = saida(num_amostras_descartadas+num_amostras_identific+1:size(entrada,1),:); %Amostras do(s) sinal(is) de saída para validação
    for k=1:1:size(curva_estat_aux,1)
        for j=1:1:dimensao_saida
            saida_LIT_est(:,j) = subs(inv_curva_estat{k,j},saida(:,j)); %Saída do sistema LIT
        end
        saida_identificacao = saida_LIT_est(num_amostras_descartadas+1:num_amostras_descartadas+num_amostras_identific,:); %Amostras do(s) sinal(is) de saída para identificação
        ordem_LIT =determina_ordem(entrada_identificacao',saida_identificacao',1); %Determina a ordem do sistema linear 
        [A{k}, B{k}, C{k}, D{k}] = moesp_po(entrada_identificacao',saida_identificacao',ordem_LIT,2); %Determina as matrizes de estados via método MOESP-PI
        A{k} = real(A{k}); B{k} = real(B{k}); C{k} = real(C{k}); D{k} = real(D{k});
        saida_LIT_identificada = dlsim(A{k}, B{k}, C{k}, D{k}, entrada_validacao);
        for i=1:1:dimensao_saida
            saida_identificada(:,i) = subs(curva_estat_sym{k,i},saida_LIT_identificada(:,i));
        end
        for i=1:1:dimensao_saida
            RMSE(k,i) = (sqrt(sum((saida_validacao - saida_identificada).^2)))/(sqrt(sum((saida_validacao - mean(saida_validacao(:,i))).^2))); %Índice RMSE
        end
    end
    %% Determinação do melhor modelo, ordem da(s) curva(s) estática(s) e matrizes de estados
    soma_RMSE = sum(RMSE,2);
    menor_RMSE = min(soma_RMSE); 
    for k=1:1:size(curva_estat_aux,1)
        if soma_RMSE(k) == menor_RMSE
            posicao = k;
            break;
        end
    end
    indice_RMSE = RMSE(posicao,:);
    A = A{posicao};
    B = B{posicao}; 
    C = C{posicao};
    D = D{posicao};
    saida_LIT_final = dlsim(A, B, C, D, entrada);
    for i=1:1:dimensao_saida
        curvas_estaticas_ident{i} = curva_estat_sym{posicao,i};
        saida_final(:,i) = subs(curvas_estaticas_ident{i},saida_LIT_final(:,i));
    end
    %% Esboço dos sinais de entrada
    caracteres = 'abcdefghijlmnopqrstuvxz';
    figure();
    for j=1:1:dimensao_entrada
        subplot(dimensao_entrada,1,j);
        plot(tempo(num_amostras_descartadas:size(entrada,1)),entrada(num_amostras_descartadas:size(entrada,1),j));  
        ylabel(['Entrada ',num2str(j)]);
        tempo_min = num_amostras_descartadas;
        tempo_max = size(entrada,1);
        y_min = 0.5*min(entrada(num_amostras_descartadas:size(entrada,1),j));
        y_max = 1.25*max(entrada(num_amostras_descartadas:size(entrada,1),j));
        axis([tempo_min tempo_max y_min y_max]);
        SP= round(((size(entrada,1)-num_amostras_descartadas)/2) + num_amostras_descartadas);
        hold on
        plot(SP,[y_min:0.01:y_max],'k');
        title(caracteres(j));
    end
    xlabel('k');
    legend('Entrada(s) do sistema');
    %% Esboço dos sinais de saída 
    figure();
    for j=1:1:dimensao_saida
        subplot(dimensao_saida,1,j);
        plot(tempo(num_amostras_descartadas:size(saida,1)),saida(num_amostras_descartadas:size(saida,1),j),'r');  
        ylabel(['Saida ',num2str(j)]);
        tempo_min = num_amostras_descartadas;
        tempo_max = size(saida,1);
        y_min = 0.5*min(saida(num_amostras_descartadas:size(saida,1),j));
        y_max = 1.25*max(saida(num_amostras_descartadas:size(saida,1),j));
        axis([tempo_min tempo_max y_min y_max]);
        SP= round(((size(saida,1)-num_amostras_descartadas)/2) + num_amostras_descartadas);
        hold on
        plot(SP,[y_min:0.01:y_max],'k');
        title(caracteres(j));
    end
    xlabel('k');
    legend('Saída(s) do sistema');
    %% Validação por simulação livre
    figure();
    for j=1:1:dimensao_saida
        subplot(dimensao_saida,1,j)
        plot(tempo(num_amostras_descartadas+num_amostras_identific:size(entrada,1)),saida_final(num_amostras_descartadas+num_amostras_identific:size(entrada,1),j),'linewidth',1.5);
        hold on
        plot(tempo(num_amostras_descartadas+num_amostras_identific:size(entrada,1)), saida(num_amostras_descartadas+num_amostras_identific:size(entrada,1),j),'r');
        ylabel(['Saída ',num2str(j)]);
        tempo_min = num_amostras_descartadas+num_amostras_identific;
        tempo_max = size(entrada,1);
        y_min = 0.5*min(saida(num_amostras_descartadas+num_amostras_identific:size(entrada,1),j));
        y_max = 1.25*max(saida(num_amostras_descartadas+num_amostras_identific:size(entrada,1),j));
        axis([tempo_min tempo_max y_min y_max]);
        title(caracteres(j));
    end
    xlabel('k');
    legend('Sistema identificado','Dados de saída do sistema');
    %% Impressão na tela da(s) curva(s) estática(s) e das matrizes de estados
    disp('Curva(s) Estatica(s)');
    for j=1:1:dimensao_saida
        pretty (curvas_estaticas_ident{j});
    end
    disp('Representação em espaço de estados');
    A 
    B 
    C
    D
end