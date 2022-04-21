%% Cálculo das curvas estáticas do modelo de Wiener
function [curva_estat_aux] = curvas_estaticas_win(entrada_estatica_win, saida_estatica_win, tempo_est) 
    ordem_max_estat = 3; %Ordem máxima das curvas estáticas
    dimensao_entrada = size(entrada_estatica_win{1},2);
    dimensao_saida = size(saida_estatica_win{1},2);
    combinacoes_win = combination([1:1:ordem_max_estat],dimensao_saida); %Possíveis combinações entre as ordens das curvas estáticas estimadas
    for j=1:1:size(saida_estatica_win,2)
        amplitude_degraus(j) = entrada_estatica_win{j}(1); %Valores dos degraus de entrada
        for k=1:1:dimensao_saida
            reg_perman{k}(j) = saida_estatica_win{j}(size(saida_estatica_win{1},1),k); %Valores de regime permanente referentes ao vetor 'amplitude_degraus'
        end
    end
    for k=1:1:dimensao_saida
        for j=1:1:ordem_max_estat
            nao_linear_win{k,j} = polyfit(amplitude_degraus,reg_perman{k},j); %Curvas estimadas (de diferentes ordens) para cada uma das saídas
            coef_aux = nao_linear_win{k,j}(1);
            ganho_LIT{k,j} = (abs(coef_aux)).^(1/j);
            passo = 0;
            for n=j+1:-1:1
                nao_linear_win{k,j}(n) = nao_linear_win{k,j}(n)/(abs(coef_aux)^((passo)/j)); %Curvas estáticas de diferentes ordens (modelo de Wiener) 
                passo = passo + 1;
            end
        end      
    end 
    for j=1:1:size(combinacoes_win,1)
        for k=1:1:dimensao_saida
            curva_estat_aux{j,k} = nao_linear_win{k,combinacoes_win(j,k)}; %Combinações entre as curvas estáticas estimadas para o modelo de Wiener
        end
    end
    %% Imprime as respostas temporais dos ensaios estáticos
    caracteres = 'abcdefghijlmnopqrstuvxz';
    figure();
    for k=1:1:dimensao_saida
        subplot(dimensao_saida,1,k);
        for i=1:1:size(saida_estatica_win,2)
            saida_estatica_aux(1:size(saida_estatica_win{1},1),i) = saida_estatica_win{i}(:,k);  
        end
        plot(tempo_est, saida_estatica_aux);
        ylabel(['Saída ',num2str(k)]);
        title(caracteres(k));
        tempo_inf = tempo_est(1);
        tempo_sup = tempo_est(size(tempo_est,1));
        y_inf = min(min(saida_estatica_aux));
        y_inf = y_inf - 0.1*y_inf;
        y_sup = max(max(saida_estatica_aux));
        y_sup = y_sup + 0.1*y_sup;
        axis([tempo_inf  tempo_sup y_inf y_sup]);
    end
    xlabel('k');
end