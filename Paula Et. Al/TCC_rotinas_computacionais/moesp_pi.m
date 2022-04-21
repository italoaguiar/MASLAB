function [A,B,C,D,SS]=moesp_pi(u,y,n,k);
% Sintaxe
        % [A,B,C,D,SS]=moesp_pi(u,y,n,k);
% Descri��o
        % O algoritmo moesp_pi estima as matrizes de modelos na representa��o espa�o de estados
        % sem tratar ru�do de processo. Tamb�m pode ser utilizado para determinar a ordem do sistema.
% Dados de Entrada
        % u -> matriz de dados medidos das entradas do sistema;
        % y -> matriz de dados medidos das sa�das do sistema;
        % n -> ordem escolhida para o modelo do sistema;
        % k -> n�mero de linhas da matriz em blocos de Hankel, este n�mero � definido como
        % k = 2(maxord/nusaida). Em que, maxord � a m�xima ordem que o usu�rio pressup�e que o
        % sistema tenha e nusaida � o n�mero de sa�das que o sistema possui. Esta rela��o � definida de
        % forma emp�rica por (Van Overschee e De Moor, 1996).
% Dados de Sa�da
        % A -> matriz din�mica estimada para o sistema em espa�o de estados;
        % B -> matriz de entrada estimada para o sistema em espa�o de estados;
        % C -> matriz de sa�da estimada para o sistema em espa�o de estados;
        % D -> matriz de transmiss�o direta estimada para o sistema em espa�o de estados;
        % SS -> matriz de valores singulares da proje��o obl�qua, utilizada para determinar a ordem
        % do sistema;
% Implementado por Rodrigo Augusto Ricco: rodrigo.ricco@yahoo.com.br
% Universidade Federal de Minas Gerais - UFMG
% �ltima modifica��o 14/11/2012 
% m=dim(u), l=dim(y), n=dim(x)
i=k;% i=numero de linhas;
% U=2im x j ; Y=2il x j 
[l,Ndados] = size(y); [m,Ndados] = size(u); 
j = Ndados-2*i; % numero de colunas
kk = 0;
for k = 1:m:2*i*m-m+1
kk = kk+1; U(k:k+m-1,:) = u(:,kk:kk+j-1); % Matriz de dados U
end
kk = 0;
for k = 1:l:2*i*l-l+1
kk = kk+1;
Y(k:k+l-1,:) = y(:,kk:kk+j-1); % Matriz de dados Y
end
Uf = U(i*m+1:2*i*m,:); % Dados de entradas futuras
Yf = Y(i*l+1:2*i*l,:); % Dados de sa�das futuras
Up = U(1:i*m,:);  % Dados de entradas passadas
Yp = Y(1:i*l,:); % Dados de sa�das passadas
Wp = [Up; Yp]; % Empilhando Up e Yp
H = [Uf; Up; Yf]; % Matriz de dados final
% Decomposi��o LQ 
L = triu(qr([H]'))';
im=i*m;
il=i*l;
L11 = L(1:im,1:im);
L21 = L(im+1:2*im,1:im);
L22 = L(im+1:2*im,im+1:2*im);
L31 = L(2*im+1:2*im+il,1:im);
L32 = L(2*im+1:2*im+il,im+1:2*im);
% Decomposi��o em valores singulares
[UU,SS,VV]=svd([L32]);
U1 = UU(:,1:n);
Oi = U1*sqrtm(SS(1:n,1:n)); % Matriz de observabilidade estendida
% Matrizes A e C 
C = Oi(1:l,1:n);
A = pinv(Oi(1:l*(i-1),1:n))*Oi(l+1:i*l,1:n);
% Matrizes B e D
U2 = UU(:,n+1:size(UU',1))';
Z = U2*[L31]/[L11];
% A partir desse ponto todos os algoritmos s�o iguais 
mm = l*i-n;
M = zeros(mm*i,m);
LL = zeros(mm*i,l+n);
for h = 1:i
    M((h-1)*mm+1:h*mm,:)=Z(:,(h-1)*m+1:h*m);
    LL((h-1)*mm+1:h*mm,:)=[U2(:,(h-1)*l+1:h*l) U2(:,h*l+1:end)*Oi(1:end-h*l,:)];
end
DB = pinv(LL)*M;
D = DB(1:l,:); 
B = DB(l+1:size(DB,1),:);