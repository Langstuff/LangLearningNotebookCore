local notebook_mt = {}
notebook_mt.__index = notebook_mt

local function new_notebook()
  local obj = {
    cells = {}
  }
  setmetatable(obj, notebook_mt)

  return obj
end

---@param code string
function notebook_mt:execute(code)
  local loaded, err = load(code, "Cell #" .. (#self.cells + 1), "t") -- todo: environment
  if not loaded then
    return nil, err
  end
  local ret, err = loaded()
  if not ret then
    return nil, err
  end
  return true, ret
end

return {
  new = new_notebook
}
