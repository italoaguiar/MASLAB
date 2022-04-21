%% Cálculo das curvas estáticas do modelo de Hammerstein
function [curva_estat_aux] = curvas_estaticas_ham(entrada_estatica_ham, saida_estatica_ham, tempo_est)
    ordem_max_estat = 3; %Ordem máxima das curvas estáticas
    dimensao_entrada = size(entrada_estatica_ham{1}{1},2);
    dimensao_saida = size(saida_estatica_ham{1}{1},2);
    combinacoes_ham = combination([1:1:ordem_max_estat],dimensao_entrada); %Possíveis combinações entre as ordens das curvas estáticas estimadas
    if dimensao_entrada > dimensao_saida
        disp('Número de entradas maior que o número de saidas'); %Número de entradas deve ser menor ou igual ao número de saídas
    end
    if dimensao_entrada <= dimensao_saida
        for n=1:1:dimensao_entrada
            for j=1:1:size(saida_estatica_ham{1},2)
                amplitude_degraus{n}(j) = entrada_estatica_ham{n}{j}(1,n); %Valores dos degraus de entrada
                for k=1:1:dimensao_saida
                    reg_perman{n}{k}(j) = saida_estatica_ham{n}{j}(size(saida_estatica_ham{1}{1},1),k); %Valores de regime permanente referentes ao vetor 'amplitude_degraus'
                end
            end
            for k=1:1:dimensao_saida
                for j=1:1:ordem_max_estat
                    nao_linear_ham{n}{k,j} = polyfit(amplitude_degraus{n},reg_perman{n}{k},j); %Curvas estimadas (de diferentes ordens) para cada par entrada-saída
                    ganho_LIT{n}(k,j) = nao_linear_ham{n}{k,j}(1);
                    nao_linear_final{n}{k,j} = nao_linear_ham{n}{k,j}/ganho_LIT{n}(k,j); %Curvas estáticas de diferentes ordens (modelo de Hammerstein)
                end
            end
        end
        for k=1:1:size(combinacoes_ham,1)
            posicao_ordem = combinacoes_ham(k,:);
            for j=1:1:dimensao_entrada
                B(1:dimensao_entrada,j) = ganho_LIT{j}(1:dimensao_entrada,posicao_ordem(j));
                C(j) = reg_perman{1}{j}(1);
                U(j) = amplitude_degraus{j}(1);
                soma_aux = nao_linear_ham{1}{j, posicao_ordem(1)}(1:posicao_ordem(1));
                soma_aux = [soma_aux 0];
                polin_aux = poly2sym(soma_aux,'z');
                soma_aux = subs(polin_aux,U(j));
                C(j) =  C(j) - soma_aux;
            end
            coefs = inv(B)*C';
            for i=1:1:dimensao_entrada
                curva_estat_aux{k,i} = [nao_linear_final{i}{i,posicao_ordem(i)}(1:posicao_ordem(i)) coefs(i)]; %Combinações entre as curvas estáticas estimadas para o modelo de Hammerstein
            end
        end      
    end
    %% Imprime as respostas temporais dos ensaios estáticos
    caracteres = 'abcdefghijlmnopqrstuvxz';
    for j=1:1:dimensao_entrada
        figure();
        for k=1:1:dimensao_saida
            subplot(dimensao_saida,1,k);
            for i=1:1:size(saida_estatica_ham{1},2)
                saida_estatica_aux(1:size(saida_estatica_ham{1}{1},1),i) = saida_estatica_ham{j}{i}(:,k);  
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
end             