function [A,B,C,D,SS]=moesp_po(u,y,n,k);
% Sintaxe
        % [A,B,C,D,SS]=moesp_po(u,y,n,k);
% Descrição
        % O algoritmo moesp_po estima as matrizes de modelos na representação 
        % espaço de estados para sistemas com ruído de processo e de medição 
        % colorido. Também pode ser utilizado para determinar a ordem do sistema.
% Dados de Entrada
        % u -> matriz de dados medidos das entradas do sistema;
        % y -> matriz de dados medidos das saídas do sistema;
        % n -> ordem escolhida para o modelo do sistema;
        % k -> número de linhas da matriz em blocos de Hankel, este número é definido como
        % k = 2(maxord/nusaida). 
        % Em que, maxord é a máxima ordem que o usuário pressupõe que o
        % sistema tenha e nusaida é o número de saídas que o sistema possui. 
        % Esta relação é definida de forma empírica por (Van Overschee e De Moor, 1996).
% Dados de Saída
        % A -> matriz dinâmica estimada para o sistema em espaço de estados;
        % B -> matriz de entrada estimada para o sistema em espaço de estados;
        % C -> matriz de saída estimada para o sistema em espaço de estados;
        % D -> matriz de transmissão direta estimada para o sistema em espaço de estados;
        % SS -> matriz de valores singulares da projeção oblíqua, utilizada para determinar a
        % ordem do sistema;
% Implementado por Rodrigo Augusto Ricco: rodrigo.ricco@yahoo.com.br
% Universidade Federal de Minas Gerais - UFMG
% Última modificação 14/11/2012 
i=k; % i=número de linhas; 
% m=dim(u), l=dim(y), n=dim(x)
% U=2im x j ; Y=2il x j 
[l,Ndados] = size(y); [m,Ndados] = size(u); j = Ndados-2*i;
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
Yf = Y(i*l+1:2*i*l,:); % Dados de saídas futuras
Up = U(1:i*m,:);       % Dados de entradas passadas
Yp = Y(1:i*l,:);       % Dados de saídas passadas
Wp = [Up; Yp];         % Empilhando Up e Yp
H = [Uf; Wp; Yf];      % Matriz de dados final
% Decomposição LQ 
L = triu(qr([H]'))';
im=i*m;
il=i*l;
L11 = L(1:im,1:im);
L21 = L(im+1:2*im,1:im);
L22 = L(im+1:2*im,im+1:2*im);
L31 = L(2*im+1:2*im+il,1:im);
L32 = L(2*im+1:2*im+il,im+1:2*im);
L33 = L(2*im+1:2*im+il,2*im+1:2*im+il);
L41 = L(2*im+il+1:2*im+2*il,1:im);
L42 = L(2*im+il+1:2*im+2*il,im+1:2*im);
L43 = L(2*im+il+1:2*im+2*il,2*im+1:2*im+il);
L44 = L(2*im+il+1:2*im+2*il,2*im+il+1:2*im+2*il);
R11 = L11;
R21 = [L21;L31];
R22 = [L22 zeros(im,il); L32 L33];
R31 = L41;
R32 = [L42 L43];
R33=L44;
% Decomposição em valores singulares
[UU,SS,VV]=svd([R32]);
U1 = UU(:,1:n);
Oi = U1*sqrtm(SS(1:n,1:n)); % Matriz de observabilidade estendida
% Matrizes A e C
C = Oi(1:l,1:n);
A = pinv(Oi(1:l*(i-1),1:n))*Oi(l+1:i*l,1:n);
% Matrizes B e D
U2 = UU(:,n+1:size(UU',1))';
Z = U2*[R31]/[R11];
% A partir desse ponto todos os algoritmos são iguais 
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