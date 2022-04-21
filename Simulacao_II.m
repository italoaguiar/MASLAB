 X = [3 3.5 4 4.5 5 5.5 7 8 9]'; %Tensão na bomba
 y = [137 227 330 424 484 550 791 961 1061]'; %Volume medido em 15s (em mL)
 y = y*4;  %Volume medido em 1 min (em mL)
 y = y/1000; %Vazão da bomba em L/min
 y = y/60;
 theta = inv(X'*X)*X'*y %Parâmetro estimado, tal que y = theta*X
 figure(1);
 plot(X, y, 'o');
 title('Vazão(dL/min) x Tensao(V)');
 xlabel('Tensão (V)');
 ylabel('Vazao(dL/min)');
 x = X';
 y_x = theta*x;
 hold on;
 plot(x, y_x,'r');
 legend('Pontos medidos','Curva estimada algoritmo');
 grid on; 
 y_x = y_x';
 RMSE = sqrt(sum((y - y_x).^2))/sqrt(sum((y - mean(y)).^2))
 
 