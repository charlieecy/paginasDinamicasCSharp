using Backend.DTO;
using Backend.Error;
using CSharpFunctionalExtensions;

namespace Backend.Service;

public interface IFunkoService
{
    Task<Result<FunkoResponseDTO, FunkoError>>  GetByIdAsync(long id);
    Task<Result<PageResponse<FunkoResponseDTO>, FunkoError>> GetAllAsync(FilterDTO filter);
    Task<Result<FunkoResponseDTO, FunkoError>> CreateAsync(FunkoPostPutRequestDTO dto);
    Task<Result<FunkoResponseDTO, FunkoError>> UpdateAsync(long id, FunkoPostPutRequestDTO dto);
    Task<Result<FunkoResponseDTO, FunkoError>> PatchAsync(long id, FunkoPatchRequestDTO dto);

    Task<Result<FunkoResponseDTO, FunkoError>>DeleteAsync(long id);
}