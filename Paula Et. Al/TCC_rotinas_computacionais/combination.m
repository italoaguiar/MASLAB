function A = combination(matriz_dim_ordem, NC) %Retorna uma tabela com todas as combinações entre as ordens das curvas estáticas 
    for j=1:1:NC
        varargin{j} = matriz_dim_ordem;
    end  
    ii = NC:-1:1;
    if NC > 1
        [A{ii}] = ndgrid(varargin{ii}) ;
        A = reshape(cat(NC+1,A{:}),[],NC) ;
    end
    if NC==1,
        A = varargin{1}(:);
    end
end

